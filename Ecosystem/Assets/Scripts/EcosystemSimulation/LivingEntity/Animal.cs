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
    public Dictionary<System.Enum, float> urge_to_run;
    public Vector2 objective;
    public List<Vector2> path;

    [Header("Navigation")]
    public NavMeshAgent agent;
    public bool isWandering;

    public override void Start()
    {
        base.Start();

        lifespan = data.lifespan.get_random_value();
        
        sensoryDistance = data.sensory_distance.get_random_value();
        speed = data.speed.get_random_value();
        gestation_duration = data.gestation_duration.get_random_value();
        number_of_children = (int)Mathf.Round(data.number_of_children.get_random_value());
        size = data.minMaxSize.x;
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed * 2;
        isWandering = true;

        hunger = data.maxHunger;
        thirst = data.maxThirst;

        urge_to_run = new Dictionary<System.Enum, float>
        {
            { AnimalType.rabbit, 0f },
            { AnimalType.fox, 0.75f },
            { PlantType.herb, 0f },
            { PlantType.oakTree, 0f}
        };

        gameObject.transform.position += new Vector3(
            0f,
            gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2,
            0f
        );
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

    public GameObject GetMostDangerousPredator()
    {
        GameObject predator = null;
        float highestFear = 0f;

        foreach (GameObject entity in GetNearbyEntities())
        {
            if (entity.GetComponent<Entity>().type is AnimalType)
            {
                if (((Animal)entity.GetComponent<Entity>().livingEntity).data.can_eat.animals.Contains((AnimalType)gameObject.GetComponent<Entity>().type))
                {
                    System.Enum entityType = (System.Enum)entity.GetComponent<Entity>().type;
                    if (urge_to_run.ContainsKey(entityType) && urge_to_run[entityType] > highestFear)
                    {
                            predator = entity;
                            highestFear = urge_to_run[entityType];
                    }
                }
            }
        }
        
        return predator;
    }

    public List<GameObject> GetNearbyFood()
    {
        return new List<GameObject>();
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

    public List<GameObject> GetNearbyEntities()
    {
        List<GameObject> nearbyEntities = new List<GameObject>();
        
        Vector3 sensoryPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(sensoryPosition, sensoryDistance);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<Entity>() != null)
            {
                nearbyEntities.Add(collider.gameObject);
            }
        }

        return nearbyEntities;
    }

    public GameObject[] GetPlantsFromTile(GameObject tile)
    {
        return tile.GetComponent<TileManager>().tilePopulation;
    }

    // À compléter
    public GameObject GetFleeingObjective()
    {
        GameObject predator = GetMostDangerousPredator();
        if (predator != null)
        {
            // Find the tile that is the furthest from the predator, in range
            GameObject furthestTile = null;
            List<GameObject> nearbyTiles = GetNearbyTiles();

            foreach (GameObject tile in nearbyTiles)
            {
                if (furthestTile == null)
                {
                    furthestTile = tile;
                }
                else
                {
                    if (Vector3.Distance(tile.transform.position, predator.transform.position) > Vector3.Distance(furthestTile.transform.position, predator.transform.position))
                    {
                        furthestTile = tile;
                    }
                }
            }

            return furthestTile.GetComponent<TileManager>().centerPlacement;
        }
        else
        {
            return null;
        }
    }
    public GameObject GetWaterObjective()
    {
        return null;
    }
    public GameObject GetFoodObjective()
    {
        return null;
    }
    public GameObject GetMateObjective()
    {
        return null;
    }
    public GameObject GetRandomObjective()
    {
        GameObject tile = GetNearbyTiles()[(int)Random.Range(0, GetNearbyTiles().Count)];
        return tile.GetComponent<TileManager>().centerPlacement;
    }

    public override void SimulationUpdate()
    {
        base.SimulationUpdate();

        /*
        Idée : Avoir tout les objectifs, pour chaque possibilité,
        et choisir l'objectif selon les statistiques de l'animal et des
        autres objectifs. Si il n'y a pas d'objectif, l'animal avance
        à une position aléatoire.
        */

        GameObject fleeingPredatorObjective = GetFleeingObjective(); // Now working
        GameObject waterObjective = GetWaterObjective(); // TODO: add isNearbyWater attribute to tiles
        GameObject foodObjective = GetFoodObjective();
        GameObject mateObjective = GetMateObjective();
        GameObject randomObjective = GetRandomObjective();

        // TODO: remove the HasReachedGoal() check, because the
        // animal has to change destination if needed
        if (fleeingPredatorObjective != null)
        {
            agent.destination = fleeingPredatorObjective.transform.position;
        }
        else if (HasReachedGoal())
        {
            // Just wander around for now
            agent.destination = randomObjective.transform.position;
        }

        hunger -= 0.1f;
        thirst -= 0.1f;
    }

    public GameObject FindObjective()
    {
        List<GameObject> nearbyTiles = GetNearbyTiles();
        
        GameObject closestFood = null;

        foreach (GameObject tile in nearbyTiles)
        {
            GameObject[] plants = GetPlantsFromTile(tile);

            foreach (GameObject plant in plants)
            {
                if (plant.GetComponent<Entity>().type == (System.Enum)PlantType.herb)
                {
                    if (closestFood == null)
                    {
                        closestFood = plant;
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, plant.transform.position) < Vector3.Distance(transform.position, closestFood.transform.position))
                        {
                            closestFood = plant;
                        }
                    }
                }
            }
        }

        if (closestFood == null)
        {
            return null;
        }
        else
        {
            return closestFood;
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