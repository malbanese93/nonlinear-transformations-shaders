using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour {

    public GameObject initPanel; 

    public GameObject optionsContainer;

    public GameObject[] options;

    public GameObject selectorPanel;

    public GameObject loadingScreen;

	// Use this for initialization
	void Start () {
        initPanel.SetActive(true);
        optionsContainer.SetActive(false);
        selectorPanel.SetActive(false);
        loadingScreen.SetActive(false);
	}
	
	public void SetLoadingScreen(bool value)
    {
        loadingScreen.SetActive(value);
    }

    public void AfterLoading()
    {
        initPanel.SetActive(false);
        optionsContainer.SetActive(true);
        selectorPanel.SetActive(true);
    }

    public void ChangeOptionPanel(int id)
    {
        // ID = number of child in options panel
        for (int i = 0; i < options.Length; ++i)
        {
            options[i].SetActive(id == i);
        }
    }
}
