using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVertexShader : MonoBehaviour {

    Material m;
    float a;

	// Use this for initialization
	void Start () {
        m = GetComponent<MeshRenderer>().material;
        a = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        a += 1.0f;

        m.SetFloat("_TwistAngle", a);
	}
}
