using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBendDropdown : MonoBehaviour {

    public GameObject mainObject;
    Dropdown dropdown;

	// Use this for initialization
	void Start () {
        dropdown = GetComponent<Dropdown>();

        ChangeValue();
	}
	
	public void ChangeValue()
    {
        mainObject.GetComponent<Renderer>().sharedMaterial.SetInt(dropdown.name, dropdown.value);
    }
}
