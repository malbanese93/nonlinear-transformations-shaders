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

    [Header("UI Elements")]
    public GameObject initPanel;
    public GameObject optionsPanel;
    public GameObject loadingScreen;
    public Dropdown[] latticeDropdowns; // in order to set all params at once

    public static readonly string DEFAULT_PATH = @"E:\unity5\Projects\LatticeTest\LatticeTest\thesis\Assets\Mesh\";

    private void Awake()
    {
        // get material reference
        material = mainObject.GetComponent<Renderer>().material;
        meshFilter = mainObject.GetComponent<MeshFilter>();
        shaderSetupScript = mainObject.GetComponent<ShaderSetupScript>();

        // Show only init message at first
        initPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }


    // NB! in order to have a simple uniform structure for all values in the menu,
    // the slider name must have the same name as the corresponding shader variable.
    // Moreover, the text value must be of the form <slider_name> + Value
    public void OnChangeSlider(Slider slider)
    {
        // change text (use only 3 significant digits)
        float val = Mathf.Round((slider.value * 100)) / 100.0f;

        Text description = slider.transform.parent.Find(slider.name + "Value").GetComponent<Text>();
        description.text = val.ToString();

        // set shader variable
        material.SetFloat(slider.name, slider.value);
    }


    public void OnBendStartDropdown(Dropdown dropdown)
    {
        material.SetInt(dropdown.name, dropdown.value);
    }

    public void OnLatticeDropdown()
    {
        shaderSetupScript.ResetGridPoints(latticeDropdowns[0].value, latticeDropdowns[1].value, latticeDropdowns[2].value);
        shaderSetupScript.Setup();
    }

    public void OnMultiLatticeToggle(Toggle toggle)
    {
        shaderSetupScript.isMultiplePointLattice = toggle.isOn;
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
        meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Use 32-bit index for vertices
        meshFilter.sharedMesh = myMesh;

        // NB: recalculate all values needed for other scripts (especially lattice!)
        mainObject.SetActive(true);
        mainObject.transform.parent.gameObject.SetActive(true); // usually meshes are child of another object when imported in Unity...
        mainObject.GetComponent<ShaderSetupScript>().Setup();

        // Hide initial message and enable options
        initPanel.SetActive(false);
        optionsPanel.SetActive(true);

        // Show screen again
        loadingScreen.SetActive(false);

        Debug.Log("Loading complete");
    }
}
