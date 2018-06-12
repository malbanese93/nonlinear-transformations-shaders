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
    Vector3[] startVertices;

    // L,M,N parameters for lattice
    // This specifies the degree of the Bezier curve along that axis
    // Remember that for degree k you have k+1 points!
    public IntVector3 gridParams;

    // Save gridpoints in local coords
    Vector3[,,] gridpointsPos;

	// Use this for initialization
	void Start () {
        print(BinomialCoefficient(70, 63));

        // First of all, retrieve bounds for mesh
        mesh = GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
        extents = bounds.extents;
        startVertices = mesh.vertices;

        // Get parameters for grid
        int L = gridParams.L;
        int M = gridParams.M;
        int N = gridParams.N;

        // Assert all values are positive
        Debug.Assert(L > 0 && M > 0 && N > 0);

        // Create lattice points
        gridpointsPos = new Vector3[L+1, M+1, N+1];
        for (int i = 0; i <= L; ++i)
            for (int j = 0; j <= M; ++j)
                for (int k = 0; k <= N; ++k)
                    GenerateGridPoint(ref gridpointsPos, i, j, k);

        // Generate lattice vertices
        GenerateVertices();
    }

    private void GenerateGridPoint(ref Vector3[,,] gridpointsPos, int i, int j, int k)
    {
        // We need to express grid points in world space
        // In order to do so, we first set them as stu (aka in percentage)...
        Vector3 stuCoords = new Vector3 { x = (float)i / gridParams.L, y = (float)j / gridParams.M, z = (float)k / gridParams.N };
        //print(stuCoords);

        //... then in local space
        gridpointsPos[i,j,k] = GetLocalCoords(stuCoords);
        //print(gridpointsPos[i, j, k]);
    }

    void ModifyLattice(GameObject controlPoint)
    {
        // Change position of vertex...
        controlPoint.transform.localPosition += new Vector3(0.0f, 0.2f, 0.0f);
        var idx = controlPoint.GetComponent<LatticeVertexScript>().index;

        // This is the index i,j,k for point P_ijk
        var i = idx.L;
        var j = idx.M;
        var k = idx.N;

        // and update grid point
        gridpointsPos[i, j, k] = controlPoint.transform.localPosition;

        // Get all mesh vertices and apply transformation
        var vertices = mesh.vertices;

        // For each mesh vertex...
        for( int v = 0; v < vertices.Length; ++v )
        {
            // 1) get STU coords
            var stuVertex = GetSTUCoords(startVertices[v]); // NB: use (s,t,u) coords of original vertices of the mesh!
            //print(stuVertex);
            float s = stuVertex.x;
            float t = stuVertex.y;
            float u = stuVertex.z;

            // 2) apply transformation to each vertex
            Vector3 newPosition = Vector3.zero;
            for (int vi = 0; vi <= gridParams.L; ++vi)
                for( int vj = 0; vj <= gridParams.M; ++vj)
                    for( int vk = 0; vk <= gridParams.N; ++vk)
                    {
                        float sBernstein = BinomialCoefficient(gridParams.L, vi) * Mathf.Pow(1 - s, gridParams.L - vi) * Mathf.Pow(s, vi);
                        float tBernstein = BinomialCoefficient(gridParams.M, vj) * Mathf.Pow(1 - t, gridParams.M - vj) * Mathf.Pow(t, vj);
                        float uBernstein = BinomialCoefficient(gridParams.N, vk) * Mathf.Pow(1 - u, gridParams.N - vk) * Mathf.Pow(u, vk);

                        newPosition += gridpointsPos[vi, vj, vk] * sBernstein * tBernstein * uBernstein;
                    }

            vertices[v] = newPosition;
        }

        // apply transformation
        mesh.vertices = vertices;
    }

    // Transform from STU coords to local coords
    Vector3 GetLocalCoords(Vector3 stuCoord)
    {
        Vector3 res = 2 * Vector3.Scale(extents, stuCoord) - extents;

        return res;
    }

    // Transform from local coords to STU coords (apply reverse transformation wrt above)
    Vector3 GetSTUCoords(Vector3 localCoords)
    {
        Vector3 res;
        res.x = (localCoords.x + extents.x) / (2.0f * extents.x);
        res.y = (localCoords.y + extents.y) / (2.0f * extents.y);
        res.z = (localCoords.z + extents.z) / (2.0f * extents.z);

        return res;
    }

    // Display a little cube for each vertex
    void GenerateVertices()
    {
        for (int i = 0; i <= gridParams.L; ++i)
            for (int j = 0; j <= gridParams.M; ++j)
                for (int k = 0; k <= gridParams.N; ++k)
                {
                    // Generate debug cube
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "Cube_" + i + "_" + j + "_" + k;

                    // Add mouse interaction script
                    cube.AddComponent<LatticeVertexScript>();
                    cube.GetComponent<LatticeVertexScript>().index = new IntVector3 { L = i, M = j, N = k };

                    // Change position and scaling
                    cube.transform.parent = transform;
                    cube.transform.localScale *= 0.1f;

                    // Set it as a child of the mesh
                    cube.transform.localPosition = gridpointsPos[i, j, k];
                    cube.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
    }

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
                    ModifyLattice(hit.transform.gameObject);
                }
            }
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
