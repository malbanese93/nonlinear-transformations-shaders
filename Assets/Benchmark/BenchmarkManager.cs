using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BenchmarkManager : MonoBehaviour {

    [Header("Mesh containers")]
    public GameObject GPUContainer, CPUContainer;
    int currentID;    

    public void Start()
    {
        currentID = 0;

        foreach (var s in GPUContainer.GetComponentsInChildren<ShaderSetupScript>(true))
            s.Setup(false);

        foreach (var s in CPUContainer.GetComponentsInChildren<CPUAnimation>(true))
            s.Setup();
    }

    public void EnableCPU(Toggle toggle)
    {
        foreach (var c in CPUContainer.GetComponentsInChildren<CPUAnimation>(true))
            c.SetEnabled(toggle.isOn);
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

        CPUContainer.transform.GetChild(currentID).gameObject.SetActive(false);
        GPUContainer.transform.GetChild(currentID).gameObject.SetActive(false);

        currentID = id;
        CPUContainer.transform.GetChild(currentID).gameObject.SetActive(true);
        GPUContainer.transform.GetChild(currentID).gameObject.SetActive(true);
    }

    public void OnChangeTransformation(Dropdown dropdown)
    {
        var id = dropdown.value;

        if (id == 0)
            return;

        foreach(var c in CPUContainer.GetComponentsInChildren<CPUAnimation>(true))
            c.SetTransformation((TransformationEnum)id);

        foreach (var c in GPUContainer.GetComponentsInChildren<GPUAnimation>(true))
            c.SetTransformation((TransformationEnum)id);

    }
}
