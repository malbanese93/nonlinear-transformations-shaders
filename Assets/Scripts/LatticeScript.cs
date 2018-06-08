﻿using System;
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
    Vector3[,,] gridpointsPos;

	// Use this for initialization
	void Start () {
        // First of all, retrieve bounds for mesh
        print(BinomialCoefficient(5, 2));
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
        for( int i = 0; i <= L; ++i )
            for( int j = 0; j <= M; ++j )
                for( int k = 0; k <= N; ++k )
                    gridpointsPos[i, j, k] = new Vector3 { x = (float)i / L, y = (float)j / M, z = (float)k / N };

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
        // Change position of vertex...
        vertexObject.transform.position += new Vector3(0.0f, 0.2f, 0.0f);
        var idx = vertexObject.GetComponent<LatticeVertexScript>().index;
        var x = idx.L;
        var y = idx.M;
        var z = idx.N;

        // and update grid point
        gridpointsPos[x, y, z] = GetSTUCoords(vertexObject.transform.position);

        // Get all mesh vertices and apply transformation
        var vertices = mesh.vertices;

        // For each mesh vertex...
        for( int v = 0; v < vertices.Length; ++v )
        {
            // 1) get STU coords
            var stuVertex = GetSTUCoords(startVertices[v]); // NB: use (s,t,u) coords of original vertices of the mesh!
            float s = stuVertex.x;
            float t = stuVertex.y;
            float u = stuVertex.z;

            // 2) apply transformation to each vertex
            Vector3 newPosition = Vector3.zero;
            for (int i = 0; i <= gridParams.L; ++i)
                for( int j = 0; j <= gridParams.M; ++j)
                    for( int k = 0; k <= gridParams.N; ++k)
                    {
                        float sBernstein = BinomialCoefficient(gridParams.L, i) * Mathf.Pow(1 - s, gridParams.L - i) * Mathf.Pow(s, i);
                        float tBernstein = BinomialCoefficient(gridParams.M, j) * Mathf.Pow(1 - t, gridParams.M - j) * Mathf.Pow(t, j);
                        float uBernstein = BinomialCoefficient(gridParams.N, k) * Mathf.Pow(1 - u, gridParams.N - k) * Mathf.Pow(u, k);

                        newPosition += GetLocalCoords(gridpointsPos[i, j, k]) * sBernstein * tBernstein * uBernstein;
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
                    // Show coordinates in STU and local systems
                    Vector3 stuCoords = gridpointsPos[i, j, k];
                    Vector3 localCoords = GetLocalCoords(stuCoords);
                    
                    //print("POINT (" + i + "," + j + "," + k + ") -- STU: " + stuCoords + " -- LC: " + localCoords);

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
                    cube.transform.localPosition = localCoords;
                }
    }

    // Calculate binomial coefficient ( n choose k ) in linear time
    private float BinomialCoefficient(int n, int k)
    {
        int kmin = Math.Min(k, n - k);
        float res = 1.0f;

        // based on the equivalence n choose k = (n/k) (n-1 choose k-1)
        for( int i = 0; i < kmin; ++i )
        {
            //print((n - i + 1) + " " + (n - k - i + 1));
            res *= (float)(n - i) / (k - i);
        }

        return res;
    }

    // Keep L,M,N >= 1 in editor!
    void OnValidate()
    {
        gridParams.L = Math.Max(1, gridParams.L);
        gridParams.M = Math.Max(1, gridParams.M);
        gridParams.N = Math.Max(1, gridParams.N);
    }
}
