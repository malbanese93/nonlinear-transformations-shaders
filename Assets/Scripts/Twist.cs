using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twist : MonoBehaviour {

    Material material;

	// Use this for initialization
	void Start () {
        Bounds b = GetComponent<MeshFilter>().mesh.bounds;

        // set max z
        material = GetComponent<Renderer>().material;
        material.SetFloat("_MaxExtent", 2 * b.extents.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
