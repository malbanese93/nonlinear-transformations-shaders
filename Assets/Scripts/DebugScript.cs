using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        print("WORKING WITH " + GetComponent<MeshFilter>().mesh.vertices.Length + " VERTICES");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
