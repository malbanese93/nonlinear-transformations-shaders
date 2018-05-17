using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexModScript : MonoBehaviour {

    public ComputeShader shader;

    private Mesh mesh;
    private Vector3[] vertices;

    void RunComputeShader()
    {
        Vector3[] output = new Vector3[vertices.Length];
        ComputeBuffer buffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);

        buffer.SetData(vertices);
        int kernel = shader.FindKernel("Multiply");
        shader.SetBuffer(kernel, "dataBuffer", buffer);

        shader.Dispatch(kernel, vertices.Length, 1, 1);
        buffer.GetData(output);

        mesh.vertices = output;
    }

	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        RunComputeShader();
    }
	
}
