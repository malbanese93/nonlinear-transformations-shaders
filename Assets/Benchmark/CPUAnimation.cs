using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUAnimation : MonoBehaviour {

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
    }

    internal void SetEnabled(bool isOn)
    {
        isTransformationEnabled = isOn;
    }

    public void SetTransformation(TransformationEnum newTransformation)
    {
        // disable current component
        if (currentTransformation != TransformationEnum.IGNORE)
        {
            // reset value for transformation (only for current mesh)
            if(scriptMap[currentTransformation].transform.gameObject.activeSelf)
                scriptMap[currentTransformation].DoTransformation(0);

            // disable
            scriptMap[currentTransformation].enabled = false;
        }

        currentTransformation = newTransformation;

        if (currentTransformation != TransformationEnum.IGNORE)
            scriptMap[currentTransformation].enabled = true;
    }

}
