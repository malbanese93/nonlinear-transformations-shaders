using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transformation : MonoBehaviour {

    protected Mesh mesh;
    protected Bounds bounds;
    protected Vector3 extents;

    protected Vector3[] startVertices;
    protected Vector3[] startNormals;

    protected CPUAnimation cpuAnimation;

	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
        extents = bounds.extents;

        startVertices = mesh.vertices;
        startNormals = mesh.normals;

        cpuAnimation  = GetComponent<CPUAnimation>();

        StartTransformation();
    }

    // Update is called once per frame
    public void Update()
    {
        if (cpuAnimation.isTransformationEnabled)
            DoTransformation(Mathf.Sin(Time.time));
    }

    abstract public void StartTransformation();
    abstract public void DoTransformation(float sineTime);
}
