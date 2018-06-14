using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSetupScript : MonoBehaviour {

    Material material;

	// Use this for initialization
	void Start () {
        // Set max extents as a uniform
        Bounds b = GetComponent<MeshFilter>().mesh.bounds; // Bounds in Local Space
        material = GetComponent<Renderer>().material;
        material.SetVector("_MaxExtents", b.extents);

        print(b.extents);
	}

	// Update is called once per frame
	void Update () {

	}
}
