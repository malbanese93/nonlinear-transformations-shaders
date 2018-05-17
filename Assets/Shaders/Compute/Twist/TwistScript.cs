using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistScript : MonoBehaviour {


    public ComputeShader shader;
    [Range(-0.05f, 0.05f)] public float alpha;  // theta = alpha * z;

    private Mesh mesh;
    private Vector3[] vertices, startVertices;
    private Vector3[] normals, startNormals;

    void RunComputeShader()
    {
        int nVertices = vertices.Length;

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
        mesh.normals = normals;
    }

	// Use this for initialization
	void Start () {
        // Check support for compute shaders
        if(!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("COMPUTE SHADERS NOT SUPPORTED!");
            Application.Quit();
        }

        // get mesh data
        mesh = GetComponent<MeshFilter>().mesh;
        startVertices = mesh.vertices;
        vertices = mesh.vertices;

        startNormals = mesh.normals;
        normals = mesh.normals;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Mouse1))
            RunComputeShader();
    }

}
