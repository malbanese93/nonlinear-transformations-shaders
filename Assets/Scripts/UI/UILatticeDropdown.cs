using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILatticeDropdown : MonoBehaviour {

    public GameObject mainObject;
    public Dropdown[] latticeDropdowns; // in order to set all params at once

    // Use this for initialization
    void Start () {
        ChangeValue();
	}
	
	public void ChangeValue()
    {
        ShaderSetupScript shaderSetupScript = mainObject.GetComponent<ShaderSetupScript>();
        shaderSetupScript.gridParams = 
            new IntVector3 { L = latticeDropdowns[0].value, M = latticeDropdowns[1].value, N = latticeDropdowns[2].value };
        shaderSetupScript.Setup();
    }
}
