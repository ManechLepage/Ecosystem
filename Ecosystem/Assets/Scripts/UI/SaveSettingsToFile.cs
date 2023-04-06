using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class SaveSettingsToFile : MonoBehaviour
{
    public string climate;
    public int waterQuantity;
    public int seed;
    public int mapSize;
    [Space]
    public TMP_Text waterText;
    public TMP_Text seedText;
    public TMP_Text mapSizeText;
    [Space]
    public Slider waterSlider;
    public Slider seedSlider;
    public Slider mapSizeSlider;


    void Start() 
    {
        waterSlider.onValueChanged.AddListener(delegate {changeSliderValue();});
        seedSlider.onValueChanged.AddListener(delegate {changeSliderValue();});
        mapSizeSlider.onValueChanged.AddListener(delegate {changeSliderValue();});
    }
    
    void Update() 
    {
        waterText.text = waterSlider.value.ToString();
        waterQuantity = (int)waterSlider.value;
        
        seedText.text = seedSlider.value.ToString();
        seed = (int)seedSlider.value;

        mapSizeText.text = mapSizeSlider.value.ToString();
        mapSize = (int)mapSizeSlider.value;

    }

    public void changeSliderValue() 
    {
    }

    public void WriteToFile()
    {
        string path = "Assets/TextFiles/SimulationSettings.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(waterQuantity);
        writer.WriteLine(seed);
        writer.WriteLine(mapSize);
        writer.Close();
    }
}
