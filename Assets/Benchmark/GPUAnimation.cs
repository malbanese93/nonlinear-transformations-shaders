using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUAnimation : MonoBehaviour {

    // oscillate values as sin(speed * t)
    float t;
    public float speed = 1.0f;

    float min;
    float max;

    public TransformationEnum currentTransformation;

    private Dictionary<TransformationEnum, string> transformationMap;

    public void Start()
    {
        transformationMap = new Dictionary<TransformationEnum, string>();

        SetTransformationMap();
    }

    private void SetTransformationMap()
    {
        transformationMap.Add(TransformationEnum.TWIST, "_TwistAngleY");
        transformationMap.Add(TransformationEnum.STRETCH, "_StretchAmountX");
        transformationMap.Add(TransformationEnum.BEND, "_BendAngleY");
        // TODO COMPLETARE
    }

    public void Update()
    {
        if (currentTransformation == TransformationEnum.IGNORE)
            return;

        print(currentTransformation.ToString() + " - " + min + " " + max);
        t = Time.time;
        GetComponent<Renderer>().material.SetFloat(transformationMap[currentTransformation], min + (max - min) * (1 + Mathf.Sin(speed * t))/2.0f );
    }

    public void SetTransformation(TransformationEnum newTransformation)
    {
        switch (newTransformation)
        {
            case TransformationEnum.TWIST:
            case TransformationEnum.BEND:
                min = -180.0f;
                max = 180.0f;
                break;

            default:
                min = -1.0f;
                max = 1.0f;
                break;
        }

        // reset old value and set new one
        if( currentTransformation != TransformationEnum.IGNORE )
            GetComponent<Renderer>().material.SetFloat(transformationMap[currentTransformation], 0.0f);

        currentTransformation = newTransformation;
    }

}
