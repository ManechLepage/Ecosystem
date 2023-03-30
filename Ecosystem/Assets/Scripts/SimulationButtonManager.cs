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

    public void ClimateButton(string Type)
    {
        foreach (Button button in climateButtons) 
        {

        }
    }
}
