using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeedSwitcherManager : MonoBehaviour
{
    public TMP_InputField textField;

    int seed;

    void Start()
    {
        seed = GameManager.instance.seed;
        UpdateSeed();
    }

    void UpdateSeed()
    {
        if (seed > GameManager.instance.maxSeed)
            seed = GameManager.instance.maxSeed;
        
        if (seed != -1)
            textField.text = seed.ToString();
        else
            textField.text = "";
        
        GameManager.instance.seed = seed;
    }

    public void UpdateSeedFromText()
    {
        if (textField.text != "")
            seed = int.Parse(textField.text);
        else
            seed = -1;
        
        UpdateSeed();
    }
}
