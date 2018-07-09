using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBenchmark : MonoBehaviour {

    public void OnBenchmarkButton()
    {
        StartCoroutine(OpenBenchmark());
    }

    IEnumerator OpenBenchmark()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BenchmarkScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
