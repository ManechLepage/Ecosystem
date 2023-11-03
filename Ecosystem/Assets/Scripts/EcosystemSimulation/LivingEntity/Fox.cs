using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Animal
{
    public override void Start()
    {        
        base.Start();
        this.urge_to_run = new Dictionary<System.Enum, float>
        {
            { AnimalType.rabbit, 0f },
            { AnimalType.fox, 0f },
            { PlantType.herb, 0f },
            { PlantType.oakTree, 0f}
        };
    }
}
