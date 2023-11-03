using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakTree : Plant
{
    public override void Start()
    {
        base.Start();
        
        type = PlantType.oakTree;
    }
}
