using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : Plant
{
    public override void Start()
    {
        growth_sizes = new List<float> { 0.6f, 1f, 1.5f };
    }
}
