using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SciptableObjectDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public List<EcosystemData> datas;
    
    // Start is called before the first frame update
    void Start()
    {
        if (dropdown != null)
        {
            dropdown.ClearOptions();
            
            if (datas.Contains(GameManager.instance.ecosystemData))
                AddScriptableToOptions(GameManager.instance.ecosystemData);

            foreach (EcosystemData data in datas)
            {
                if (data != GameManager.instance.ecosystemData)
                {
                    AddScriptableToOptions(data);
                }
            }
        }
    }

    void AddScriptableToOptions(EcosystemData data)
    {
        List<string> newOptions = new List<string>();
        newOptions.Add(data.name);

        dropdown.AddOptions(newOptions);
    }

    public void ChangedValue()
    {
        foreach (EcosystemData data in datas)
        {
            if (data.name == dropdown.options[dropdown.value].text)
            {
                GameManager.instance.ecosystemData = data;
            }
        }
    }
}
