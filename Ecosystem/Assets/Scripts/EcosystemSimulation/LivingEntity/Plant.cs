using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : LivingEntity
{
    public PlantData data;

    public override void Start()
    {
        base.Start();

        lifespan = data.lifespan.get_random_value();
        size = 1f;
    }
}
