using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Animal
{
    public override void Start()
    {        
        base.Start();

        type = AnimalType.rabbit;

        this.urge_to_run = new Dictionary<System.Enum, float>
        {
            { AnimalType.rabbit, 0f },
            { AnimalType.fox, 0.75f },
            { PlantType.herb, 0f },
            { PlantType.oakTree, 0f}
        };
    }
}
