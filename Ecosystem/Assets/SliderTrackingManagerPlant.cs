using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTrackingManagerPlant : MonoBehaviour
{
    public PlantType plant;
    public Slider slider;
    public TextMeshProUGUI text;

    void Start()
    {
        SetInitialValue();
    }

    public void UpdateText()
    {
        text.text = slider.value.ToString();
        UpdatePopulation();
    }

    PlantPopulation GetCorrespondingPopulation()
    {
        foreach (PlantPopulation pop in GameManager.instance.ecosystemData.populations.plantPopulations)
        {
            if (pop.type == plant)
            {
                return pop;
            }
        }

        PlantPopulation population = new PlantPopulation();
        population.type = plant;
        GameManager.instance.ecosystemData.populations.plantPopulations.Add(population);
        return population;
    }
    
    void UpdatePopulation()
    {
        PlantPopulation population = GetCorrespondingPopulation();
        population.population = (int)slider.value;
    }

    void SetInitialValue()
    {
        PlantPopulation population = GetCorrespondingPopulation();
        slider.value = (float)population.population;
        Debug.Log(slider.value);
        UpdateText();
    }
}
