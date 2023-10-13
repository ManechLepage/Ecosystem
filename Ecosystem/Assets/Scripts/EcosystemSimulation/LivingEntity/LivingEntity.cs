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

    public void Awake()
    {
        StartCoroutine(SimulationLoop());
    }

    IEnumerator SimulationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            SimulationUpdate();
        }
    }

    public virtual void SimulationUpdate()
    {
        age += 0.1f;
    }

    public virtual void Start()
    {
        
        age = 0f;
    }

    public void convertGridToWorldPosition()
    {

    }
}
