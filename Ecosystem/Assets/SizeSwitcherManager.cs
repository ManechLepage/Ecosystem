using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SizeSwitcherManager : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    int size;

    void Start()
    {
        size = (int)GameManager.instance.size.x;
        UpdateData();
    }

    public void ChangedSize()
    {
        if (size < 128)
            size += 32;
        else
            size = 32;
        UpdateData();
    }

    void UpdateData()
    {
        textMesh.text = $"{size} x {size}";
        GameManager.instance.size = new Vector2(size, size);
    }
}
