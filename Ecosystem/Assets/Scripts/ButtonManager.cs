using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    public Button terrainButton;
    public Button livingBeingButton;
    [Space]
    public GameObject terrain;
    public GameObject livingBeing;

    private bool isTerrainButtonSelected = true;

    void Start() 
    {
        TerrainButton();
    }
    
    public void TerrainButton()
    {
        if (!isTerrainButtonSelected) 
        {
            isTerrainButtonSelected = false;
            terrain.SetActive(true);
            livingBeing.SetActive(false);
            Debug.Log("Terrain");
        }
    }

    public void LivingBeingButton()
    {
        if (isTerrainButtonSelected) 
        {
            isTerrainButtonSelected = true;
            livingBeing.SetActive(true);
            terrain.SetActive(false);
            Debug.Log("LivingBeing");
        }
    }
}
