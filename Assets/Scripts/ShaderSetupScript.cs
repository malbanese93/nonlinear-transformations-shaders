using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSetupScript : MonoBehaviour {

    Material material;

	// Use this for initialization
	void Start () {
        //Bounds b = GetComponent<Renderer>().bounds; // Bounds in World Space
        Bounds b = GetComponent<MeshFilter>().mesh.bounds; // Bounds in Local Space

        // set max z
        material = GetComponent<Renderer>().material;

        // TODO: check if extents must be fixed along y axis
        material.SetVector("_MaxExtents", b.extents);

        print(b.extents);
	}

	// Update is called once per frame
	void Update () {

	}
}
