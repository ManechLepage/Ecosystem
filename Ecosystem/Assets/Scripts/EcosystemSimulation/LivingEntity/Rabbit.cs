using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Animal
{
    public override void Start()
    {        
        base.Start();
        
        this.lifespan = data.lifespan.get_random_value();
        this.size = 1f;
        this.sensory_distance = data.sensory_distance.get_random_value();
        this.speed = data.speed.get_random_value();
        this.gestation_duration = data.gestation_duration.get_random_value();
        this.number_of_children = (int)Mathf.Round(data.number_of_children.get_random_value());
        this.desirability = 0.5f;

        this.urge_to_run = new Dictionary<System.Enum, float>
        {
            { AnimalType.rabbit, 0f },
            { AnimalType.fox, 0.75f },
            { PlantType.herb, 0f },
            { PlantType.oakTree, 0f}
        };
    }
}
