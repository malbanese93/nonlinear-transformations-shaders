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

    // Compute shader data
    public ComputeShader computeShader;
    ComputeBuffer buffer;
    int kernelID;

    // Params for transformation
    [Range(-0.5f, 0.5f)] public float alpha;  // theta = alpha * z;

    // Mesh info
    private Mesh mesh;
    VertexData[] vData;
    int vCount;

    // Number of blocks dispatched
    static readonly int numberOfThreadGroups = 128;

	// Use this for initialization
	void Start ()
    {
        // Check support for compute shaders
        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("COMPUTE SHADERS NOT SUPPORTED!");
            Application.Quit();
        }

        // Get mesh reference
        mesh = GetComponent<MeshFilter>().mesh;
        vCount = mesh.vertexCount;
        //print(vCount);

        // The only dependence we need to consider is the one between pos_i and n_i for a given vertex i.
        // The vertices are independent among themselves (embarassingly parallel)

        // Thus we restructure data in order to work on (pos_i, n_i) data.
        RestructureMeshData(mesh.vertices, mesh.normals);

        // Setup kernel
        kernelID = SetupKernel();
    }

    // Create buffer, send it to the GPU and initialize kernel
    private int SetupKernel()
    {
        // Set buffer to send to the GPU
        buffer = new ComputeBuffer(vCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexData)));
        buffer.SetData(vData);

        // Set kernel info
        int kernel = computeShader.FindKernel("Twist");
        computeShader.SetFloat("alpha", alpha);
        computeShader.SetBuffer(kernel, "g_buffer", buffer);
        computeShader.SetInt("vertexCount", vCount);

        // Set buffer and vertex count in vertex shader
        Material m = GetComponent<MeshRenderer>().material;
        m.SetBuffer("buffer", buffer);

        // return kernel id
        return kernel;
    }

    // Save all data as an array of (pos_i, n_i)
    private void RestructureMeshData(Vector3[] v, Vector3[] n)
    {
        vData = new VertexData[vCount];

        for (int i = 0; i < vCount; ++i)
        {
            vData[i] = new VertexData { pos = v[i], normal = n[i] };
        }
    }

    void RunComputeShader()
    {
        // Update alpha value
        computeShader.SetFloat("alpha", alpha);

        // Dispatch all threads in a 1D fashion
        computeShader.Dispatch(kernelID, numberOfThreadGroups, 1, 1);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            
            RunComputeShader();
        }
    }

    private void OnDestroy()
    {
        // Mark CPU buffer as disposable for GC
        buffer.Dispose();
    }

}
