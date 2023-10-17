using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : LivingEntity
{
    public PlantData data;
    public int stageIndex;
    public int meshTypeIndex;

    public override void Start()
    {
        base.Start();

        // meshTypeIndex = 0;
        meshTypeIndex = (int)Random.Range(0, data.meshes.Count);
        gameObject.GetComponent<Renderer>().materials = data.materials;
        stageIndex = 1;

        // gameObject.transform.rotation = Quaternion.Euler(0f, 0f, (int)Random.Range(0, 360));
        // print(gameObject.transform.rotation.z);

        lifespan = data.lifespan.get_random_value();
        SetMesh();
    }

    public override void SimulationUpdate()
    {
        base.SimulationUpdate();

        if (age > stageIndex * data.stageIncrement)
        {
            stageIndex++;            
            
            if (stageIndex >= data.meshes[meshTypeIndex].meshes.Count)
            {
                stageIndex = 1;
                age = 0f;
            }
            SetMesh();
        }

        
    }

    public void SetMesh()
    {
        gameObject.GetComponent<MeshFilter>().mesh = data.meshes[meshTypeIndex].meshes[stageIndex - 1];
        
    }
}
