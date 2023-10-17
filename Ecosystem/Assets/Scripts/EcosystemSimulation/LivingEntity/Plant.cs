using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : LivingEntity
{
    public PlantData data;
    public int stageIndex = 1;
    public int meshTypeIndex;
    public float stageIncrement;

    public override void Start()
    {
        base.Start();

        meshTypeIndex = (int)Random.Range(0, data.meshes.Count);
        gameObject.GetComponent<Renderer>().materials = data.materials;

        lifespan = data.lifespan.get_random_value();
        gameObject.GetComponent<MeshFilter>().mesh = data.meshes[meshTypeIndex].meshes[stageIndex];
    }

    public override void SimulationUpdate()
    {
        base.SimulationUpdate();

        if (age > stageIndex * stageIncrement)
        {
            stageIndex++;
            GetComponent<MeshFilter>().mesh = data.meshes[meshTypeIndex].meshes[stageIndex - 1];
        }
    }
}
