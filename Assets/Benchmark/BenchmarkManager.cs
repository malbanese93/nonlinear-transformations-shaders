using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BenchmarkManager : MonoBehaviour {

    [Header("Meshes (0 ignored!)")]
    public Mesh[] meshes;

    [Header("Mesh Holders")]
    public GameObject CPUObj, GPUObj;

    public void Start()
    {
        GPUObj.GetComponent<ShaderSetupScript>().Setup();
    }

    public void EnableCPU(Toggle toggle)
    {
        CPUObj.GetComponent<CPUAnimation>().SetEnabled(toggle.isOn);
    }

    public void OnBackButton()
    {
        StartCoroutine(BackToMenu());
    }

    IEnumerator BackToMenu()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void OnChangeMesh(Dropdown dropdown)
    {
        var id = dropdown.value;

        // blank state
        if (id == 0)
            return;

        CPUObj.GetComponent<MeshFilter>().mesh = meshes[id];

        GPUObj.GetComponent<MeshFilter>().mesh = meshes[id];
        GPUObj.GetComponent<ShaderSetupScript>().Setup();
    }

    public void OnChangeTransformation(Dropdown dropdown)
    {
        var id = dropdown.value;

        if (id == 0)
            return;

        CPUObj.GetComponent<CPUAnimation>().SetTransformation((TransformationEnum)id);
        GPUObj.GetComponent<GPUAnimation>().SetTransformation((TransformationEnum)id);
    }
}
