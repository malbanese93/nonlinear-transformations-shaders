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
        public int x;
        public int y;
        public int z;
    }

    Bounds bounds;
    Vector3 extents;

    public IntVector3 numberOfGridPoints;
    Vector3[,,] gridpoints;

	// Use this for initialization
	void Start () {
        // First of all, retrieve bounds for mesh
        bounds = GetComponent<MeshFilter>().mesh.bounds;
        extents = bounds.extents;

        // Create lattice points
        int L = numberOfGridPoints.x;
        int M = numberOfGridPoints.y;
        int N = numberOfGridPoints.z;
        gridpoints = new Vector3[L+1, M+1, N+1];

        for( int i = 0; i <= L; ++i )
            for( int j = 0; j <= M; ++j )
                for( int k = 0; k <= N; ++k )
                    gridpoints[i, j, k] = new Vector3 { x = (float)i / L, y = (float)j / M, z = (float)k / N };

        DebugPoints();

        // Translate and scale axes so that:
        // 1) it is centered on the lowest corner
        // 2) all values are normalized
        // (S,T,U) coordinates


    }

    // Transform from STU coords to local coords
    Vector3 GetLocalCoords(Vector3 stuCoord)
    {
        return 2 * Vector3.Scale(extents, stuCoord) - extents;
    }

    // Print values in (S,T,U) and local coord systems
    void DebugPoints()
    {
        for (int i = 0; i <= numberOfGridPoints.x; ++i)
            for (int j = 0; j <= numberOfGridPoints.y; ++j)
                for (int k = 0; k <= numberOfGridPoints.z; ++k)
                    print("POINT (" + i + "," + j + "," + k + ") = " + gridpoints[i, j, k] + " -- LC: " + GetLocalCoords(gridpoints[i, j, k]));
    }
}
