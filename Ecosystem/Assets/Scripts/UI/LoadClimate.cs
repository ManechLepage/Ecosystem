using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadClimate : MonoBehaviour
{
    public GameObject simulationGenerator;
    public TMP_Text climateText;

    void Update()
    {
        climateText.text = simulationGenerator.GetComponent<SaveSettingsToFile>().climate;
    }
}
