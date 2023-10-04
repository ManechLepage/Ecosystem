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
    public SimulationManager simulation;
    public Vector2 gridPosition;
    public BellCurve base_lifespan;
    public float lifespan;
    public float age;
    public string objectName;
    public List<float> growth_sizes;
    public List<Mesh> growth_stages;
    public System.Enum type; // a faire plus tard

    public void simulationUpdate(float delta_time)
    {
        age += delta_time;
    }

    public virtual void Start()
    {
        
    }

    public void convertGridToWorldPosition()
    {

    }
}
