using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ObjImporter OBJimporter = new ObjImporter();

        // TODO: MESH SELECT WINDOW
        Mesh m = OBJimporter.ImportFile(@"E:\unity5\Projects\LatticeTest\LatticeTest\thesis\Assets\Mesh\mesh_onelayer.obj");
        GetComponent<MeshFilter>().mesh = m;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
