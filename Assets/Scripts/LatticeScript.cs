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
    Bounds bounds;
    Vector3 extents;

    // L,M,N parameters for lattice
    // NB: the number of vertices along each axis is 1 + param!
    public IntVector3 gridParams;
    Vector3[,,] gridpoints;

	// Use this for initialization
	void Start () {
        // First of all, retrieve bounds for mesh
        bounds = GetComponent<MeshFilter>().mesh.bounds;
        extents = bounds.extents;

        // Create lattice points
        int L = gridParams.L;
        int M = gridParams.M;
        int N = gridParams.N;
        gridpoints = new Vector3[L+1, M+1, N+1];

        for( int i = 0; i <= L; ++i )
            for( int j = 0; j <= M; ++j )
                for( int k = 0; k <= N; ++k )
                    gridpoints[i, j, k] = new Vector3 { x = (float)i / L, y = (float)j / M, z = (float)k / N };

        // Check if all points are generated correctly
        DebugPoints();

    }

    // Transform from STU coords to local coords
    Vector3 GetLocalCoords(Vector3 stuCoord)
    {
        Vector3 res = 2 * Vector3.Scale(extents, stuCoord) - extents;

        // TODO: We are assuming that all meshes will start count along the y-axis from the bottom.
        // Be careful!
        res.y += extents.y;

        return res;
    }

    // Display debug information on lattice vertices
    void DebugPoints()
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

                    // Generate debug cube in that position
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = transform;
                    cube.name = "Cube";
                    cube.transform.position = localCoords;
                }
    }
}
