using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Animal
{
    public override void Start()
    {        
        lifespan = data.lifespan.get_random_value();
        size = 1f; //this.growth_sizes[0];
        sensory_distance = data.sensory_distance.get_random_value();
        speed = data.speed.get_random_value();
        gestation_duration = data.gestation_duration.get_random_value();
        number_of_children = (int)Mathf.Round(data.number_of_children.get_random_value());
        desirability = 0.5f;

        urge_to_run = new Dictionary<System.Enum, float> 
        {
            { AnimalType.rabbit, 0f },
            { AnimalType.fox, 0.75f },
            { PlantType.herb, 0f },
            { PlantType.oakTree, 0f}
        };

        base.Start();
    }
}
