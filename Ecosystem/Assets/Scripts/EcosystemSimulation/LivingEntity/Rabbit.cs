using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Animal
{
    void Start()
    {
        base_lifespan = new BellCurve(5, 0.25f);
        lifespan = base_lifespan.get_random_value();
        growth_sizes = new List<float> { 0.2f, 0.5f, 0.8f, 0.9f, 1f };
        size = 1f; //this.growth_sizes[0];
        sensory_distance = 5f;
        speed = 1f;
        gestation_duration = 50f;
        base_number_of_childs = new BellCurve(3, 0.5f);
        number_of_childs = (int)Mathf.Round(this.base_number_of_childs.get_random_value());
        desirability = 0.5f;

        urge_to_run = new Dictionary<EntityType, float> {
            { EntityType.rabbit, 0f },
            { EntityType.fox, 0.75f },
            { EntityType.herb, 0f },
            { EntityType.oakTree, 0f}
        };

        can_eat = new Food();
        can_eat.plants = new List<EntityType>() { EntityType.herb };
        can_eat.animals = new List<EntityType>();
    }
}
