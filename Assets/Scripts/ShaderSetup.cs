using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSetup : MonoBehaviour {

    Material material;

	// Use this for initialization
	void Start () {
        Bounds b = GetComponent<MeshFilter>().mesh.bounds;

        // set max z
        material = GetComponent<Renderer>().material;

        // TODO: check if extents must be fixed along y axis
        material.SetVector("_MaxExtents", b.extents);
	}

	// Update is called once per frame
	void Update () {

	}
}
