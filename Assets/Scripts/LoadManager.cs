using Crosstales.FB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.Elenesski.Camera.Utilities;

public class LoadManager : MonoBehaviour {
    
    [Header("Main Mesh")]
    public GameObject mainObject;
    MeshFilter meshFilter;
    ShaderSetupScript shaderSetupScript;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Test Pointlight")]
    public Light pointLight;

    public static readonly string DEFAULT_PATH = @"E:\unity5\Projects\LatticeTest\LatticeTest\thesis\Assets\Mesh\";

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
        GameObject.FindGameObjectWithTag("GameController").GetComponent<PanelManager>().SetLoadingScreen(true);

        // In order to let Unity redraw the GUI, we need to skip a frame first before importing the mesh...
        yield return null;

        // Import mesh 
        Mesh myMesh = FastObjImporter.Instance.ImportFile(path);

        mainObject.SetActive(true);
        mainObject.transform.parent.gameObject.SetActive(true); // usually meshes are child of another object when imported in Unity..

        // Set mesh
        meshFilter = mainObject.GetComponent<MeshFilter>();
        meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Use 32-bit index for vertices
        meshFilter.sharedMesh = myMesh;

        // Do all calculations required by shader
        shaderSetupScript = mainObject.GetComponent<ShaderSetupScript>();
        shaderSetupScript.Setup(true);

        // Change position of camera and light
        mainCamera.GetComponent<CameraPosition>().ChangeCamera(meshFilter.sharedMesh.bounds);
        pointLight.GetComponent<LightPosition>().ChangeLight(meshFilter.sharedMesh.bounds);

        // change screens
        GameObject.FindGameObjectWithTag("GameController").GetComponent<PanelManager>().AfterLoading();

        // Show screen again
        GameObject.FindGameObjectWithTag("GameController").GetComponent<PanelManager>().SetLoadingScreen(false);

        Debug.Log("Loading complete");
    }

}
