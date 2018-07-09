using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

    public Text counter;

	// Use this for initialization
	void Start () {
        StartCoroutine(UpdateCounter());
	}

    // Update is called once per frame
    IEnumerator UpdateCounter()
    {
        while (true)
        {
            string ms = (Time.deltaTime * 1000).ToString("00.00");
            string fps = (1.0f / Time.deltaTime).ToString("00.00");

            counter.text = "Time: " + ms + "ms\n(" + fps + " FPS)";

            yield return new WaitForSeconds(0.5f);
        }
    }
}
