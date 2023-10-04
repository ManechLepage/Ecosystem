using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    herb,
    oakTree,
    rabbit,
    fox
}

public class LivingEntity : MonoBehaviour
{
    public SimulationManager simulation;
    public Vector2 gridPosition;
    public BellCurve base_lifespan;
    public float lifespan;
    public float age;
    public string objectName;
    public List<float> growth_sizes;
    public List<Mesh> growth_stages;
    public EntityType type; // a faire plus tard

    public void simulationUpdate(float delta_time)
    {
        age += delta_time;
    }

    public void Start()
    {
        
    }

    public void convertGridToWorldPosition()
    {

    }
}
