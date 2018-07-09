using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Shader setup script 
// Mainly used to setup lattice, but also to set mesh extents used by all
public class ShaderSetupScript : MonoBehaviour {

    // Bound values for mesh
    Mesh mesh;
    Bounds bounds;
    Vector3 extents;
    Material material;

    // L,M,N parameters for lattice
    // This specifies the degree of the Bezier curve along that axis
    // Remember that for degree k you have k+1 points!
    public IntVector3 gridParams;

    // Save gridpoints in local coords
    Vector4[] gridpointsPos;

    // Is multipoint lattice enabled?
    [HideInInspector] public bool isMultiplePointLattice;

    public void Setup(bool showGrid)
    {
        // Create lattice points
        // Notice that we must use 1D arrays instead of 3D arrays
        // since there's no way to pass it to the shader otherwise!
        // NB: HLSL does not support dynamically sized arrays.
        // Everything is clamped to max values set.
        // IF you change this value, remember to change the equivalent value in the vertex shader!
        gridpointsPos = new Vector4[256];

        // NB: the center may be everywhere. Always use bound center to do all calculations
        // independently of how the mesh was created.
        material = GetComponent<Renderer>().sharedMaterial;

        // Retrieve bounds for mesh
        mesh = GetComponent<MeshFilter>().sharedMesh;
        bounds = mesh.bounds;
        extents = bounds.extents;

        // send data to shader
        material.SetVector("_BoundsCenter", bounds.center);
        material.SetVector("_MaxExtents", extents);

        // Set all initial values to default
        // TODO

        // Set lattice points
        StartLattice(showGrid);
    }

    private void StartLattice(bool showGrid)
    {
        // Delete old vertices if present
        DeleteLatticeVertices();

        ResetGridPoints();

        // Generate lattice vertices if required
        if(showGrid)
            GenerateGrid();

        // Set uniforms to shader
        material.SetInt("_L", gridParams.L);
        material.SetInt("_M", gridParams.M);
        material.SetInt("_N", gridParams.N);
        material.SetVectorArray("_ControlPoints", gridpointsPos);
    }

    private int To1DArrayCoords(int x, int y, int z)
    {
        // WIDTH * HEIGHT * z (the plane we start with) + WIDTH * y (the row we start with) + x (offset)
        // in this case WIDTH = L+1, HEIGHT = M+1
        return x + (gridParams.L+1) * (y + (gridParams.M+1) * z);
    }

    // Display a little cube for each vertex
    void GenerateGrid()
    {
        for (int i = 0; i <= gridParams.L; ++i)
            for (int j = 0; j <= gridParams.M; ++j)
                for (int k = 0; k <= gridParams.N; ++k)
                {
                    // Generate cube
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "P_" + i + "_" + j + "_" + k;

                    // Add mouse interaction script
                    cube.AddComponent<LatticeVertexScript>();
                    cube.GetComponent<LatticeVertexScript>().index = new IntVector3 { L = i, M = j, N = k };

                    // Change position and scaling
                    cube.transform.parent = transform;
                    // scaling is relative to parent size. There is no big or small cube in absolute terms, it depends on the mesh!
                    cube.transform.localScale = Vector3.one * 0.30f * Mathf.Min(Mathf.Min(extents.x, extents.y), extents.z);

                    cube.transform.localPosition = gridpointsPos[To1DArrayCoords(i, j, k)] + new Vector4(bounds.center.x, bounds.center.y, bounds.center.z, 1.0f);
                    cube.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    // set material
                    cube.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));

                    // Disable shadows for these objects
                    cube.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    cube.GetComponent<Renderer>().receiveShadows = false;
                }
    }

    private void DeleteLatticeVertices()
    {
        // Delete all children with lattice vertex script
        var latticeScripts = GetComponentsInChildren<LatticeVertexScript>();

        foreach (var script in latticeScripts)
            Destroy(script.gameObject);
    }

    public void ResetGridPoints()
    {
        for (int i = 0; i <= gridParams.L; ++i)
            for (int j = 0; j <= gridParams.M; ++j)
                for (int k = 0; k <= gridParams.N; ++k)
                {
                    // We need to express grid points in world space
                    // In order to do so, we first set them as stu (aka in percentage)...
                    Vector4 stuCoords = new Vector4 { x = (float)i / gridParams.L, y = (float)j / gridParams.M, z = (float)k / gridParams.N, w = 1 };

                    //... then in local space centered in bounds center
                    gridpointsPos[To1DArrayCoords(i,j,k)] = GetBoundCenterCoords(stuCoords);
                } 
    }

    public void ModifyLattice(GameObject controlPoint, Vector3 translationVector)
    {
        // Change position of vertex...
        controlPoint.transform.Translate(translationVector, Space.World);

        // Index for P_ijk
        var idx = controlPoint.GetComponent<LatticeVertexScript>().index;
        var i = idx.L;
        var j = idx.M;
        var k = idx.N;

        // and update grid point
        gridpointsPos[To1DArrayCoords(i, j, k)] = controlPoint.transform.localPosition - bounds.center;

        // if multipoint lattice is enabled, apply transformation for every other point at the same quota
        // (aka pj = j)
        if (isMultiplePointLattice)
        {
            for (int pi = 0; pi <= gridParams.L; ++pi)
                for(int pk = 0; pk <= gridParams.N; ++pk)
                {
                    if (pi == i && pk == k) continue; // do not translate twice!

                    var siblingCube = transform.Find("P_" + pi + "_" + j + "_" + pk);
                    siblingCube.Translate(translationVector, Space.World);
                    gridpointsPos[To1DArrayCoords(pi, j, pk)] = siblingCube.transform.localPosition - bounds.center;
                }
        }

        // Do not forget to update data on GPU!
        material.SetVectorArray("_ControlPoints", gridpointsPos);
    }

    // Transform from STU coords to local coords (centered in bounds center)
    Vector4 GetBoundCenterCoords(Vector4 stuCoord)
    {
        Vector4 res = 2 * Vector3.Scale(extents, stuCoord - 0.5f * Vector4.one);
        res.w = 1;

        return res;
    }

    private void Update()
    {
        // Reset everything when pressing R
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartLattice(true);
        }
    }

}
