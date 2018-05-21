using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lattice modifier similar to what can be found in Blender.
// Mainly based on "Free-form deformation of solid geometric models" by Sederberg, Thomas W.; Parry, Scott R. (1986).
public class LatticeScript : MonoBehaviour {

    [System.Serializable]
    public struct IntVector3
    {
        public int L;
        public int M;
        public int N;
    }

    // Bound values for mesh
    Mesh mesh;
    Bounds bounds;
    Vector3 extents;

    // L,M,N parameters for lattice
    // NB: the number of vertices along each axis is 1 + param!
    public IntVector3 gridParams;
    Vector3[,,] gridpoints;

    // Check if mesh origin is not on center but below
    public bool isCenterBottom = true;

	// Use this for initialization
	void Start () {
        // First of all, retrieve bounds for mesh
        mesh = GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
        extents = bounds.extents;

        // Get parameters for grid
        int L = gridParams.L;
        int M = gridParams.M;
        int N = gridParams.N;

        // Assert all values are positive
        Debug.Assert(L > 0 && M > 0 && N > 0);

        // Create lattice points
        gridpoints = new Vector3[L+1, M+1, N+1];
        for( int i = 0; i <= L; ++i )
            for( int j = 0; j <= M; ++j )
                for( int k = 0; k <= N; ++k )
                    gridpoints[i, j, k] = new Vector3 { x = (float)i / L, y = (float)j / M, z = (float)k / N };

        // Generate lattice vertices
        GenerateVertices();
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
                    // Change lattice vertex and then all mesh vertices
                    ModifyLattice(hit.transform.gameObject);   
                }
            }
        }
    }

    void ModifyLattice(GameObject vertexObject)
    {
        // TODO: just translate for now on click
        vertexObject.transform.position += new Vector3(0.0f, 0.2f, 0.0f);
        var P_ijk_prime = vertexObject.transform.position;

        // Get all mesh vertices and apply transformation
        var vertices = mesh.vertices;

        // For each mesh vertex...
        for( int v = 0; v < vertices.Length; ++v )
        {
            // TODO
            // 1) get STU coords
            // 2) apply transformation to each vertex
        }

        // apply transformation
        mesh.vertices = vertices;
    }

    // Transform from STU coords to local coords
    Vector3 GetLocalCoords(Vector3 stuCoord)
    {
        Vector3 res = 2 * Vector3.Scale(extents, stuCoord) - extents;

        // Add offset if center is on the bottom of the mesh 
        if(isCenterBottom)
            res.y += extents.y;

        return res;
    }

    // TODO Check correctness!
    // Transform from local coords to STU coords (apply reverse transformation wrt above)
    Vector3 GetSTUCoords(Vector3 localCoords)
    {
        Vector3 res;
        res.x = (localCoords.x + extents.x) / (2.0f * extents.x);
        res.y = (isCenterBottom) ? (localCoords.y / (2.0f * extents.y)) : (localCoords.y + extents.y) / (2.0f * extents.y);
        res.z = (localCoords.z + extents.z) / (2.0f * extents.z);

        return res;
    }

    // Display a little cube for each vertex
    void GenerateVertices()
    {
        // Show coordinates in STU and local systems
        for (int i = 0; i <= gridParams.L; ++i)
            for (int j = 0; j <= gridParams.M; ++j)
                for (int k = 0; k <= gridParams.N; ++k)
                {
                    // Show coordinates in STU and local systems
                    Vector3 localCoords = GetLocalCoords(gridpoints[i, j, k]);
                    Vector3 stuCoords = gridpoints[i, j, k];

                    print("POINT (" + i + "," + j + "," + k + ") -- STU: " + stuCoords + " -- LC: " + localCoords);

                    // Generate debug cube
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "Cube_" + i + "_" + j + "_" + k;

                    // Add mouse interaction script
                    cube.AddComponent<LatticeVertexScript>();

                    // Change position and scaling
                    cube.transform.parent = transform;
                    cube.transform.localScale *= 0.2f;

                    // Set it as a child of the mesh
                    cube.transform.position = localCoords;
                }
    }
}
