using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakTree : Plant
{
    void Start()
    {
        growth_sizes = new List<float> { 0.7f, 1.1f, 1.6f };
    }
}
