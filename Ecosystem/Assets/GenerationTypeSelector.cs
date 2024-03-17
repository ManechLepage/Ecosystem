using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GenerationType
{
    PreDefined,
    Custom
}


public class GenerationTypeSelector : MonoBehaviour
{
    public GenerationType generatorType = GenerationType.PreDefined;
    public TextMeshProUGUI textMesh;
    [Space]
    public GameObject preDefinedObject;
    public GameObject customObject;
    [Space]
    public EcosystemData customData;

    private EcosystemData currentPreDefinedData;

    void Start()
    {
        currentPreDefinedData = GameManager.instance.ecosystemData;
        preDefinedObject.SetActive(true);
        customObject.SetActive(false);
    }
    
    public void ChangedType()
    {
        if (generatorType == GenerationType.PreDefined)
            generatorType = GenerationType.Custom;
        else
            generatorType = GenerationType.PreDefined;

        UpdateData();
    }

    void UpdateData()
    {
        textMesh.text = generatorType == GenerationType.PreDefined ? "Pré-défini" : "Personnalisé";
        if (generatorType == GenerationType.Custom)
        {
            currentPreDefinedData = GameManager.instance.ecosystemData;
            GameManager.instance.ecosystemData = customData;
            preDefinedObject.SetActive(false);
            customObject.SetActive(true);
        }
        else
        {
            GameManager.instance.ecosystemData = currentPreDefinedData;
            preDefinedObject.SetActive(true);
            customObject.SetActive(false);
        }
    }
}