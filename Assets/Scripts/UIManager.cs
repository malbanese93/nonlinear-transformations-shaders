using Crosstales.FB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour {
    
    [Header("Main Mesh")]
    public GameObject mainObject;
    Material material;
    MeshFilter meshFilter;
    ShaderSetupScript shaderSetupScript;

    [Header("Panels")]
    public GameObject initPanel;
    public GameObject optionsPanel;
    public GameObject loadingScreen;

    public static readonly string DEFAULT_PATH = @"E:\unity5\Projects\LatticeTest\LatticeTest\thesis\Assets\Mesh\";

    private void Awake()
    {
        // Show only init message at first
        initPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void OnLoadMeshButton()
    {
        StartCoroutine(ImportMesh());
    }

    private IEnumerator ImportMesh()
    {
        // Open file loader (only obj!)
        string extensions = "obj";
        string path = FileBrowser.OpenSingleFile("Open File", DEFAULT_PATH, extensions);
        Debug.Log("Selected file: " + path);

        // return if no file was selected
        if (path.Equals(""))
            yield break;

        // Hide screen with loading screen
        loadingScreen.SetActive(true);

        // In order to let Unity redraw the GUI, we need to skip a frame first before importing the mesh...
        yield return null;

        // Import mesh and set it
        Mesh myMesh = FastObjImporter.Instance.ImportFile(path);

        mainObject.SetActive(true);
        mainObject.transform.parent.gameObject.SetActive(true); // usually meshes are child of another object when imported in Unity..
                                                                
        material = mainObject.GetComponent<Renderer>().material;
        meshFilter = mainObject.GetComponent<MeshFilter>();
        meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Use 32-bit index for vertices
        meshFilter.sharedMesh = myMesh;

        shaderSetupScript = mainObject.GetComponent<ShaderSetupScript>();
        shaderSetupScript.Setup();

        // Hide initial message and enable options
        initPanel.SetActive(false);
        optionsPanel.SetActive(true);

        // Set all initial values to shader
        // per ogni slider nell'options panel => chiama set slider bla bla 
        


        // Show screen again
        loadingScreen.SetActive(false);

        Debug.Log("Loading complete");
    }

}
