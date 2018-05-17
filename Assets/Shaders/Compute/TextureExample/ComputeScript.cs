using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeScript : MonoBehaviour {

    public ComputeShader shader;
    RenderTexture tex;

	void Run()
    {
        int kernelHandle = shader.FindKernel("CSMain");

        tex = new RenderTexture(256, 256, 24);
        tex.enableRandomWrite = true;
        tex.Create();

        shader.SetTexture(kernelHandle, "Result", tex);
        shader.SetFloat("time", Time.time);
        shader.Dispatch(kernelHandle, 32, 32, 1);
    }

    void Update()
    {
        Run();

        //StartCoroutine(WaitFrame());

        gameObject.GetComponent<Renderer>().material.mainTexture = tex;
    }

    private IEnumerator WaitFrame()
    {
        yield return new WaitForEndOfFrame();
    }
}
