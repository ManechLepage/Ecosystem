using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTrackingManagerQuantity : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;

    void Start()
    {
        SetInitialValue();
    }

    public void UpdateText()
    {
        text.text = slider.value.ToString() + " %";
        UpdateQuantity();
    }

    void UpdateQuantity()
    {
        GameManager.instance.ecosystemData.plantDensity = slider.value / 100f;
    }

    void SetInitialValue()
    {
        slider.value = GameManager.instance.ecosystemData.plantDensity * 100f;
        UpdateText();
    }
}
