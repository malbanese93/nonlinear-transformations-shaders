using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Crosstales.FB;

public class MeshLoader : MonoBehaviour
{
    public GameObject meshObject; // mesh used for visualization
    MeshFilter meshFilter; // .. and its meshfilter

    bool isMeshLoaded;

    public static readonly string DEFAULT_PATH = @"E:\unity5\Projects\LatticeTest\LatticeTest\thesis\Assets\Mesh\";

    public Mesh testMesh;

    // GUI Handling
    public Button loadMeshBtn;

    private void Start()
    {
        meshFilter = meshObject.GetComponent<MeshFilter>();
    }

    public void LoadMeshFromFile()
    {
        // Open file loader (only obj!)
        string extensions = "obj";
        string path = FileBrowser.OpenSingleFile("Open File", DEFAULT_PATH, extensions);
        Debug.Log("Selected file: " + path);

        // return if no file was selected
        if (path.Equals(""))
            return;

        // Import mesh and set it
        Mesh myMesh = FastObjImporter.Instance.ImportFile(path);
        meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Use 32-bit index for vertices
        meshFilter.sharedMesh = myMesh;

        // NB: recalculate all values needed for other scripts (especially lattice!)
        meshObject.GetComponent<ShaderSetupScript>().Setup();

        Debug.Log("Loading complete");

        loadMeshBtn.gameObject.SetActive(true);
    }

}