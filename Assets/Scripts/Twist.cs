using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twist : MonoBehaviour {

    Mesh mesh;
    Vector3[] originalVertices;
    Vector3[] originalNormals;

    // Use this for initialization
    void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        
        originalVertices = mesh.vertices;
        originalNormals = mesh.normals;
    }
	
	// Update is called once per frame
	void Update () {
        mesh.MarkDynamic();

        /*Vector3[] vertices = originalVertices;
        Vector3[] normals = originalNormals;

        int i = 0;
        while (i < mesh.vertices.Length)
        {
            vertices[i] += normals[i] * Mathf.Sin(Time.time) * 0.01f;
            i++;
        }

        mesh.vertices = vertices;*/
    }
}
