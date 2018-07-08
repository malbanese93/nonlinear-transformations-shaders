using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUAnimation : MonoBehaviour {

    // oscillate values as sin(speed * t)
    float t;
    public float speed = 1.0f;
    float min;
    float max;

    public bool isTransformationEnabled;

    public TransformationEnum currentTransformation;

    private Dictionary<TransformationEnum, Transformation> scriptMap;

    public void Setup()
    {
        isTransformationEnabled = false;

        scriptMap = new Dictionary<TransformationEnum, Transformation>();
        scriptMap.Add(TransformationEnum.TWIST, GetComponent<Twist>());
        scriptMap.Add(TransformationEnum.STRETCH, GetComponent<Stretch>());
        scriptMap.Add(TransformationEnum.BEND, GetComponent<Bend>());

        print(transform.name);
    }

    internal void SetEnabled(bool isOn)
    {
        isTransformationEnabled = isOn;
    }

    public void SetTransformation(TransformationEnum newTransformation)
    {
        // disable current component
        if(currentTransformation != TransformationEnum.IGNORE)
            scriptMap[currentTransformation].enabled = false;

        // enable new
        switch (newTransformation)
        {
            case TransformationEnum.TWIST:
                min = -180.0f;
                max = 180.0f;
                break;

            case TransformationEnum.BEND:
                min = -120.0f;
                max = 120.0f;
                break;

            default:
                min = -1.0f;
                max = 1.0f;
                break;
        }

        currentTransformation = newTransformation;

        if (currentTransformation != TransformationEnum.IGNORE)
            scriptMap[currentTransformation].enabled = true;
    }

}
