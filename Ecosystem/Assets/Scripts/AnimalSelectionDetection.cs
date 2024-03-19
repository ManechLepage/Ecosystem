using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalSelectionDetection : MonoBehaviour
{
    public bool isAnAnimalSelected = false;
    public GameObject animalUIPanel;

    [Space]
    [Header("Animal UI Panel")]
    public GameObject animalNameObject;
    public GameObject animalObjective;
    public Image animalHungerBar;
    public Image animalThirstBar;
    public GameObject animalReproduction;
    [Space]
    [Header("Colors")]
    public Color color1;
    public Color color2;


    private GameObject currentAnimal;

    private void Awake()
    {
        animalUIPanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject.tag == "Animal")
                {
                    Debug.Log("Animal selected");
                    isAnAnimalSelected = true;
                    animalUIPanel.SetActive(true);
                    UpdateAnimalPanel(hitObject);
                    currentAnimal = hitObject;
                }
                else
                {
                    isAnAnimalSelected = false;
                }
                animalUIPanel.SetActive(isAnAnimalSelected);
            }
        }

        if (isAnAnimalSelected)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isAnAnimalSelected = false;
                animalUIPanel.SetActive(false);
            }
            UpdateAnimalPanel(currentAnimal);
        }
    }

    string GetAnimalObjective(Animal animal)
    {
        string name;
        switch (animal.currentObjective)
        {
            case ObjectiveType.Fleeing:
            {
                name = "S'enfuir";
                break;
            }
            case ObjectiveType.Water:
            {
                name = "Boire";
                break;
            }
            case ObjectiveType.Food:
            {
                name = "Manger";
                break;
            }
            case ObjectiveType.Mate:
            {
                name = "Se reproduire";
                break;
            }
            default:
            {
                name = "Se déplacer";
                break;
            }
        }

        return name;
    }

    public string GetAnimalReproductionState(Animal animal)
    {
        string label;
        if (animal.isPregnant)
        {
            label = $"Enceinte ({animal.currentGestation.ToString()} sur {animal.gestation_duration.ToString()} jours)";
        }
        else
        {
            if (animal.canReproduce)
            {
                label = "Peut se reproduire";
            }
            else
            {
                label = "Ne peut pas se reproduire";
            }
        }
        return label;
    }

    public void UpdateAnimalPanel(GameObject animal)
    {
        Animal animalScript = animal.GetComponent<Animal>();
        animalNameObject.GetComponent<TextMeshProUGUI>().text = animalScript.data.objectName[0].ToString().ToUpper() + animalScript.data.objectName.Substring(1);
        animalObjective.GetComponent<TextMeshProUGUI>().text = GetAnimalObjective(animalScript);
        animalReproduction.GetComponent<TextMeshProUGUI>().text = GetAnimalReproductionState(animalScript);
        
        animalHungerBar.rectTransform.sizeDelta = new Vector2(animalScript.hunger / animalScript.data.maxHunger * 200f, 22f);
        animalThirstBar.rectTransform.sizeDelta = new Vector2(animalScript.thirst / animalScript.data.maxThirst * 200f, 22f);

        animalHungerBar.color = InterpolateColor(animalScript.hunger / animalScript.data.maxHunger);
        animalThirstBar.color = InterpolateColor(animalScript.thirst / animalScript.data.maxThirst);
    }

    public Color InterpolateColor(float t)
    {
        t = Mathf.Clamp01(t);

        float r = Mathf.Lerp(color1.r, color2.r, t);
        float g = Mathf.Lerp(color1.g, color2.g, t);
        float b = Mathf.Lerp(color1.b, color2.b, t);

        return new Color(r, g, b, 1f);
    }
}

