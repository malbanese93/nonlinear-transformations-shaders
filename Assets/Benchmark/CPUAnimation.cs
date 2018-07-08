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

    public TransformationEnum currentTransformation;

    public void Start()
    {
        
    }

    public void Update()
    {
        if (currentTransformation == TransformationEnum.IGNORE)
            return;

        t = Time.time;

        switch(currentTransformation)
        {
            case TransformationEnum.TWIST:
                GetComponent<MegaTwist>().angle = min + (max - min) * (1 + Mathf.Sin(speed * t)) / 2.0f;
                break;

            default:
                break;
        }
        
    }

    public void SetTransformation(TransformationEnum newTransformation)
    {
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
    }

}
