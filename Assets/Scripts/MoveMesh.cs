using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMesh : MonoBehaviour {

    public float speed = 100.0f;
    float rotation = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        rotation += speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotation, 0, rotation);
	}
}
