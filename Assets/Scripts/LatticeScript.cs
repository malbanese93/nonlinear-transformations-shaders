using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lattice modifier similar to what can be found in Blender.
// Mainly based on "Free-form deformation of solid geometric models" by Sederberg, Thomas W.; Parry, Scott R. (1986).
public class LatticeScript : MonoBehaviour {

    // Bound values for mesh
    Mesh mesh;
    Bounds bounds;
    Vector3 extents;
    Material material;

    // Check if mesh has origin on bottom
    public bool isOriginDown;

    // L,M,N parameters for lattice
    // This specifies the degree of the Bezier curve along that axis
    // Remember that for degree k you have k+1 points!
    public IntVector3 gridParams;

    // Save gridpoints in local coords
    Vector4[] gridpointsPos;

	// Use this for initialization
	void Start ()
    {
        // First of all, retrieve bounds for mesh
        mesh = GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
        extents = bounds.extents;

        // Create lattice points
        // Notice that we must use 1D arrays instead of 3D arrays
        // since there's no way to pass it to the shader otherwise!
        // NB: HLSL does not support dynamically sized arrays.
        // Everything is clamped to max values set.
        // IF you change this value, remember to change the equivalent value in the vertex shader!
        gridpointsPos = new Vector4[256];

        // Set lattice points
        StartLattice();
    }

    private void StartLattice()
    {
        // Delete old vertices if present
        DeleteLatticeVertices();

        // Reset all coordinates of the grid points
        ResetGridPoints(gridParams.L, gridParams.M, gridParams.N);

        // Generate lattice vertices
        GenerateGrid();

        // Set uniforms to shader
        material = GetComponent<Renderer>().material;
        material.SetInt("_IsOriginDown", isOriginDown == true ? 1 : 0); // SetBool does not exist!
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
                    // Generate debug cube
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "P_" + i + "_" + j + "_" + k;

                    // Add mouse interaction script
                    cube.AddComponent<LatticeVertexScript>();
                    cube.GetComponent<LatticeVertexScript>().index = new IntVector3 { L = i, M = j, N = k };

                    // Change position and scaling
                    cube.transform.parent = transform;
                    cube.transform.localScale *= 10.0f;

                    // Set it as a child of the mesh
                    cube.transform.localPosition = gridpointsPos[To1DArrayCoords(i, j, k)];
                    cube.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
    }

    private void DeleteLatticeVertices()
    {
        // Delete all children with lattice vertex script
        var latticeScripts = GetComponentsInChildren<LatticeVertexScript>();

        foreach (var script in latticeScripts)
            Destroy(script.gameObject);
    }

    private void ResetGridPoints(int L, int M, int N)
    {
        for (int i = 0; i <= L; ++i)
            for (int j = 0; j <= M; ++j)
                for (int k = 0; k <= N; ++k)
                {
                    // We need to express grid points in world space
                    // In order to do so, we first set them as stu (aka in percentage)...
                    Vector4 stuCoords = new Vector4 { x = (float)i / gridParams.L, y = (float)j / gridParams.M, z = (float)k / gridParams.N, w = 1 };

                    //... then in local space
                    gridpointsPos[To1DArrayCoords(i,j,k)] = GetLocalCoords(stuCoords);
                }

        // Set the rest of the values to zero, since they will be unused

    }

    public void ModifyLattice(GameObject controlPoint)
    {
        // Change position of vertex...
        var idx = controlPoint.GetComponent<LatticeVertexScript>().index;

        // This is the index i,j,k for point P_ijk
        var i = idx.L;
        var j = idx.M;
        var k = idx.N;

        // and update grid point
        gridpointsPos[To1DArrayCoords(i, j, k)] = controlPoint.transform.localPosition;

        // Do not forget to update data on GPU!
        material.SetVectorArray("_ControlPoints", gridpointsPos);

        // Get all mesh vertices and apply transformation
        var vertices = mesh.vertices;

        // For each mesh vertex...
        /*for( int v = 0; v < vertices.Length; ++v )
        {
            // 1) get STU coords
            float s = stuVertices[v].x;
            float t = stuVertices[v].y;
            float u = stuVertices[v].z;

            // 2) apply transformation to each vertex
            Vector3 newPosition = Vector3.zero;
            for (int pi = 0; pi <= gridParams.L; ++pi)
                for( int pj = 0; pj <= gridParams.M; ++pj)
                    for( int pk = 0; pk <= gridParams.N; ++pk)
                    {
                        float sBernstein = BinomialCoefficient(gridParams.L, pi) * Mathf.Pow(1 - s, gridParams.L - pi) * Mathf.Pow(s, pi);
                        float tBernstein = BinomialCoefficient(gridParams.M, pj) * Mathf.Pow(1 - t, gridParams.M - pj) * Mathf.Pow(t, pj);
                        float uBernstein = BinomialCoefficient(gridParams.N, pk) * Mathf.Pow(1 - u, gridParams.N - pk) * Mathf.Pow(u, pk);

                        newPosition += gridpointsPos[pi, pj, pk] * sBernstein * tBernstein * uBernstein;
                    }

            vertices[v] = newPosition;
        }*/

        // apply transformation
        mesh.vertices = vertices;
    }

    // Transform from STU coords to local coords
    Vector4 GetLocalCoords(Vector4 stuCoord)
    {
        Vector4 res = 2 * Vector3.Scale(extents, stuCoord) - extents;
        res.w = 1;

        //adjust if origin is down
        if (isOriginDown) res.y += extents.y;

        return res;
    }

    /*// Transform from local coords to STU coords (apply reverse transformation wrt above)
    Vector3 GetSTUCoords(Vector3 localCoords)
    {
        //adjust if origin is down
        if (isOriginDown) localCoords.y -= extents.y;

        Vector3 res;
        res.x = (localCoords.x + extents.x) / (2.0f * extents.x);
        res.y = (localCoords.y + extents.y) / (2.0f * extents.y);
        res.z = (localCoords.z + extents.z) / (2.0f * extents.z);

        return res;
    }*/

    /*
    // Calculate binomial coefficient ( n choose k ) in linear time
    private float BinomialCoefficient(int n, int k)
    {
        k = Math.Min(k, n - k);
        float res = 1.0f;

        // based on the equivalence n choose k = (n/k) (n-1 choose k-1)
        for( int i = 0; i < k; ++i )
            res *= (float)(n - i) / (k - i);

        return res;
    }
    */

    private void Update()
    {
        // Check if we hit a cube with the mouse
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                LatticeVertexScript latticeVertex = hit.transform.gameObject.GetComponent<LatticeVertexScript>();
                if (latticeVertex != null)
                {
                    // Change lattice control point and then all mesh vertices
                    //ModifyLattice(hit.transform.gameObject);
                }
            }
        }

        // Reset everything when pressing R
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartLattice();
        }
    }

    // Keep L,M,N >= 1 in editor!
    void OnValidate()
    {
        gridParams.L = Math.Max(1, gridParams.L);
        gridParams.M = Math.Max(1, gridParams.M);
        gridParams.N = Math.Max(1, gridParams.N);
    }
}
