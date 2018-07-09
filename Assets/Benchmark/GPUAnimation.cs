using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUAnimation : MonoBehaviour {

    float maxValue;

    public TransformationEnum currentTransformation;

    private Dictionary<TransformationEnum, string> transformationMap;

    public void Setup()
    {
        transformationMap = new Dictionary<TransformationEnum, string>();

        SetTransformationMap();
    }

    private void SetTransformationMap()
    {
        transformationMap.Add(TransformationEnum.TWIST, "_TwistAngleZ");
        transformationMap.Add(TransformationEnum.STRETCH, "_StretchAmountZ");
        transformationMap.Add(TransformationEnum.BEND, "_BendAngleY");
    }

    public void Update()
    {
        if (currentTransformation == TransformationEnum.IGNORE)
            return;

        GetComponent<Renderer>().material.SetFloat(transformationMap[currentTransformation], Mathf.Sin(Time.time) * maxValue );
    }

    public void SetTransformation(TransformationEnum newTransformation)
    {
        GetComponent<ShaderSetupScript>().Setup(false);

        switch (newTransformation)
        {
            case TransformationEnum.TWIST:
                maxValue = 90.0f;
                break;

            case TransformationEnum.BEND:
                maxValue = 60.0f;
                break;

            case TransformationEnum.STRETCH:
            default:
                maxValue = 1.0f;
                break;
        }

        if (currentTransformation != TransformationEnum.IGNORE)
        {
            Material material = GetComponent<Renderer>().material;
            string key = transformationMap[currentTransformation];
            material.SetFloat(key, 0.0f);
        }

        // ADDITIONAL values only for BEND
        if (currentTransformation != TransformationEnum.BEND)
        {
            GetComponent<Renderer>().material.SetFloat("_BendYMin", 0.0f);
            GetComponent<Renderer>().material.SetFloat("_BendYMax", 1.0f);
            GetComponent<Renderer>().material.SetFloat("_BendY0", 1.0f); // half mesh
        }

        if(currentTransformation != TransformationEnum.STRETCH )
            GetComponent<Renderer>().material.SetFloat("_StretchStrengthZ", 1.0f);

        currentTransformation = newTransformation;
    }

}
