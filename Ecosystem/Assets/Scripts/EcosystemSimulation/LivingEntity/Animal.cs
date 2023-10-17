using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Gender 
{
    Male,

    Female
}


public class Animal : LivingEntity
{
    public AnimalData data;
    public float speed;
    public float size;
    public float sensoryDistance;
    public float gestation_duration;
    public float hunger;
    public float thirst;
    public int number_of_children;
    public float reproductive_urge;
    public Gender gender;
    public float desirability;
    public Dictionary<System.Enum, float> urge_to_run;
    public Vector2 objective;
    public List<Vector2> path;

    [Header("Navigation")]
    public NavMeshAgent agent;
    public bool isWandering;
    public SphereCollider sensoryCollider;

    public override void Start()
    {
        base.Start();

        lifespan = data.lifespan.get_random_value();
        
        sensoryDistance = data.sensory_distance.get_random_value();
        speed = data.speed.get_random_value();
        gestation_duration = data.gestation_duration.get_random_value();
        number_of_children = (int)Mathf.Round(data.number_of_children.get_random_value());
        desirability = 0.5f;
        size = data.minMaxSize.x;
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed * 2;
        isWandering = true;
    }

    // public Animal reproduce(Animal partner) // pas encore testé
    // {
    //     // create an instance of the same animal as the parents
    //     System.Type parent_type = this.GetType();
    //     System.Reflection.ConstructorInfo constructor = parent_type.GetConstructor(new System.Type[] { typeof(SimulationManager) });

    //     Animal child = (Animal)constructor.Invoke(new object[] { this.simulation });
    //     child.position = this.position;

    //     // inherit the characteristics of the parents
    //     BellCurve speed_curve = new BellCurve((this.speed + partner.speed) / 2, 0.25f);
    //     child.speed = speed_curve.get_random_value();

    //     foreach (KeyValuePair<System.Type, float> entry in this.urge_to_run)
    //     {
    //         BellCurve urge_curve = new BellCurve((this.urge_to_run[entry.Key] + partner.urge_to_run[entry.Key]) / 2, 0.25f);
    //         child.urge_to_run[entry.Key] = urge_curve.get_random_value();
    //     }

    //     return child;
    // }

    public void SetRandomGoal()
    {
        GameObject target = GetNearbyTiles()[(int)Random.Range(0, GetNearbyTiles().Count)];
        agent.destination = target.GetComponent<TileManager>().centerPlacement.transform.position;
    }

    public List<GameObject> GetNearbyTiles()
    {
        List<GameObject> nearbyTiles = new List<GameObject>();
        
        Vector3 sensoryPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(sensoryPosition, sensoryDistance);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Tile")
            {
                nearbyTiles.Add(collider.gameObject);
            }
        }

        return nearbyTiles;
    }

    public override void SimulationUpdate()
    {
        base.SimulationUpdate();
        
        if (HasReachedGoal())
        {
            if (isWandering)
            {
                SetRandomGoal();
            }
        }
    }

    private bool HasReachedGoal()
    {
        return !agent.pathPending && agent.remainingDistance < 2f;
    }

}

[System.Serializable]
public class Food
{
    public List<PlantType> plants = new List<PlantType>();
    public List<AnimalType> animals = new List<AnimalType>();
}