using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour {

    public GameObject mainObject;
    Slider slider;

    public int initialValue;

	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();

        ResetSlider();
        ChangeValue();
	}
	
	public void ChangeValue()
    {
        // change text (use only 3 significant digits)
        float val = Mathf.Round((slider.value * 100)) / 100.0f;

        Text description = slider.transform.parent.Find(slider.name + "Value").GetComponent<Text>();
        description.text = val.ToString();

        // set shader variable
        mainObject.GetComponent<Renderer>().sharedMaterial.SetFloat(slider.name, slider.value);
    }

    public void ResetSlider()
    {
        slider.value = initialValue;
    }
}
