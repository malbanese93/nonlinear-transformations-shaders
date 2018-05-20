using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        print("WORKING WITH " + GetComponent<MeshFilter>().mesh.vertices.Length + " VERTICES");

        Bounds b = GetComponent<MeshFilter>().mesh.bounds;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = b.max;

        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube2.transform.position = b.min;

        print((b.max - b.min).z);
        print(b.extents.z);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
