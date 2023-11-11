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
    
    public float age;
    public float lifespan;

    public virtual void SimulationUpdate(int days)
    {
        // 2 days per second
        age += 1 / 365.25f * (float)days;
    }

    public virtual void Start()
    {
        
        age = 0f;
    }
}
