using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BenchmarkManager : MonoBehaviour {

    public GameObject GPUMeshContainer;

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

    public void Start()
    {
        // initialize all meshes with gpu transformation
        foreach(var s in GPUMeshContainer.GetComponentsInChildren<ShaderSetupScript>())
        {
            s.Setup();
            print(s.transform.parent.name);
        }
    }
}
