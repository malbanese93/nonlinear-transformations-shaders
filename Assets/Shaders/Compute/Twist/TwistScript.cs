using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistScript : MonoBehaviour {

    public struct VertexData
    {
        public Vector3 pos;
        public Vector3 normal;
    }

    public ComputeShader twistShader;
    [Range(-0.05f, 0.05f)] public float alpha;  // theta = alpha * z;

    // Mesh info
    private Mesh mesh;
    VertexData[] vData;

    void RunComputeShader()
    {
        /*int nVertices = vertices.Length;

        ComputeBuffer vBuffer = new ComputeBuffer(nVertices, sizeof(float) * 3);
        vBuffer.SetData(vertices);

        ComputeBuffer nBuffer = new ComputeBuffer(nVertices, sizeof(float) * 3);
        nBuffer.SetData(normals);

        int kernel = shader.FindKernel("Twist");
        shader.SetBuffer(kernel, "vBuffer", vBuffer);
        shader.SetBuffer(kernel, "nBuffer", nBuffer);
        shader.SetFloat("alpha", alpha);

        shader.Dispatch(kernel, nVertices, 1, 1);
        vBuffer.GetData(vertices);
        vBuffer.Dispose();

        nBuffer.GetData(normals);
        nBuffer.Dispose();

        mesh.vertices = vertices;
        mesh.normals = normals;*/
    }

	// Use this for initialization
	void Start () {
        // Check support for compute shaders
        if(!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("COMPUTE SHADERS NOT SUPPORTED!");
            Application.Quit();
        }

        // Get mesh reference
        mesh = GetComponent<MeshFilter>().mesh;

        // The only dependence we need to consider is the one between pos_i and n_i for a given vertex i.
        // The vertices are independent among themselves (embarassingly parallel)

        // Thus we restructure data in order to work on (pos_i, n_i) data.
        RestructureMeshData(mesh.vertices, mesh.normals);
    }

    // Save all data as an array of (pos_i, n_i)
    private void RestructureMeshData(Vector3[] v, Vector3[] n)
    {
        int vCount = mesh.vertexCount;
        vData = new VertexData[vCount];

        for (int i = 0; i < vCount; ++i)
        {
            vData[i] = new VertexData { pos = v[i], normal = n[i] };
        }
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Mouse1))
            RunComputeShader();
    }

}
