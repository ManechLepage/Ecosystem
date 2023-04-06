using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationButtonManager : MonoBehaviour
{
    public Button terrainButton;
    public Button livingBeingButton;
    [Space]
    public GameObject terrain;
    public GameObject livingBeing;
    [Space]
    [Space]
    [Space]
    public List<Button> climateButtons = new List<Button>();
    public List<string> climateString = new List<string>();

    private bool isTerrainButtonSelected;

    void Start() 
    {
        TerrainButton();
        foreach (Button button in climateButtons)
        {
            button.interactable = true;
        }
    }
    
    public void TerrainButton()
    {
        if (!isTerrainButtonSelected) 
        {
            isTerrainButtonSelected = true;
            terrain.SetActive(true);
            livingBeing.SetActive(false);
        }
    }

    public void LivingBeingButton()
    {
        if (isTerrainButtonSelected) 
        {
            isTerrainButtonSelected = false;
            livingBeing.SetActive(true);
            terrain.SetActive(false);
        }
    }

    public void ClimateButton(Button clickedButton)
    {
        foreach (Button button in climateButtons)
        {
            button.interactable = true;
        }

        GetComponent<SaveSettingsToFile>().climate = climateString[climateButtons.IndexOf(clickedButton)];
        clickedButton.interactable = false;
    }
}