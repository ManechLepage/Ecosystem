using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalType
{
    rabbit,
    fox
}
public enum PlantType
{
    herb,
    oakTree
}

public class LivingEntity : MonoBehaviour
{
    public Vector2 gridPosition;
    
    public float lifespan;
    public float age;
    public float size;


    public void simulationUpdate(float delta_time)
    {
        age += delta_time;
    }

    public virtual void Start()
    {
        age = 0f;
    }

    public void convertGridToWorldPosition()
    {

    }
}
