using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleLattice : MonoBehaviour {

    public GameObject mainObject;
    Toggle toggle;

	// Use this for initialization
	void Start () {
        toggle = GetComponent<Toggle>();

        ChangeValue();
	}
	
	public void ChangeValue()
    {
        mainObject.GetComponent<ShaderSetupScript>().isMultiplePointLattice = toggle.isOn;
    }
}
