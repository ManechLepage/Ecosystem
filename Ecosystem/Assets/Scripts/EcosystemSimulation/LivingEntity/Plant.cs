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
        gameObject.GetComponent<Renderer>().materials = data.meshes[meshTypeIndex].materials;

        Quaternion initalRotation = data.meshes[meshTypeIndex].initialRotation;
        gameObject.transform.rotation = Quaternion.Euler(
            0f + initalRotation.eulerAngles.x,
            Random.Range(0f, 360f) + initalRotation.eulerAngles.y,
            0f + initalRotation.eulerAngles.z
        );

        lifespan = data.lifespan.get_random_value();
        immortal = data.immortal;

        age = Random.Range(0f, lifespan);
        stageIndex = (int)Random.Range(1, data.meshes[meshTypeIndex].meshes.Count);
        SetMesh();

        float scale_variation = 1f;
        if (data.minMaxSizeRange.x != 0f || data.minMaxSizeRange.y != 0f)
            scale_variation = Random.Range(data.minMaxSizeRange.x, data.minMaxSizeRange.y);

        gameObject.transform.localScale *= data.meshes[meshTypeIndex].scale * scale_variation;

        if (gameObject.transform.localScale.x == 0f)
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (!data.meshes[meshTypeIndex].isCenterAnchored)
        {
            gameObject.transform.position += new Vector3(
                0f,
                gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2f,
                0f);
        }
    }

    public override void SimulationUpdate(int days)
    {
        base.SimulationUpdate(days);

        if (age > stageIndex * data.stageIncrement)
        {
            stageIndex++;
            
            // Maybe better idea to keep the last stage instead of resetting to stage 1
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
        if (stageIndex >= 1)
        {
            gameObject.GetComponent<MeshFilter>().mesh = data.meshes[meshTypeIndex].meshes[stageIndex - 1];
        }
    }

    public float Eat()
    {
        stageIndex--;
        SetMesh();
        return data.nutritionPerStage;
    }

    public bool CanBeEaten()
    {
        return stageIndex >= data.minStageToBeEaten;
    }
}
