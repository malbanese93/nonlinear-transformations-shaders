using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour {

    public GameObject mainObject;
    Material material; // to set values to shader

    private void Awake()
    {
        material = mainObject.GetComponent<Renderer>().material;
    }


    // NB! in order to have a simple uniform structure for all values in the menu,
    // the slider name must have the same name as the corresponding shader variable.
    // Moreover, the text value must be of the form <slider_name> + Value
    public void OnChangeSlider(Slider slider)
    {
        // change text (use only 3 significant digits)
        float val = Mathf.Round((slider.value * 100)) / 100.0f;

        Text description = slider.transform.parent.Find(slider.name + "Value").GetComponent<Text>();
        description.text = val.ToString();

        // set shader variable
        material.SetFloat(slider.name, slider.value);
    }

    // This returns the enum value of the starting value for selected bend.
    // 0 -> set equal to min value
    // 1 -> set halfway between min and max value
    // 2 -> set equal to max value
    public void OnBendStartDropdown(Dropdown dropdown)
    {
        material.SetInt(dropdown.name, dropdown.value);
    }
}
