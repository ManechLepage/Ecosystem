using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTrackingManagerAnimal : MonoBehaviour
{
    public AnimalType animal;
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

    AnimalPopulation GetCorrespondingPopulation()
    {
        foreach (AnimalPopulation pop in GameManager.instance.ecosystemData.populations.animalPopulations)
        {
            if (pop.type == animal)
            {
                return pop;
            }
        }

        AnimalPopulation population = new AnimalPopulation();
        population.type = animal;
        GameManager.instance.ecosystemData.populations.animalPopulations.Add(population);
        return population;
    }
    
    void UpdatePopulation()
    {
        AnimalPopulation population = GetCorrespondingPopulation();
        population.population = (int)slider.value;
    }

    void SetInitialValue()
    {
        AnimalPopulation population = GetCorrespondingPopulation();
        slider.value = (float)population.population;
        Debug.Log(slider.value);
        UpdateText();
    }
}
