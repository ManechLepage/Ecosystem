using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Gender 
{
    Male,

    Female
}

[System.Serializable]
public enum ObjectiveType
{
    Fleeing,
    Water,
    Food,
    Mate,
    Random
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
    public ObjectiveType currentObjective;

    [Header("Navigation")]
    public NavMeshAgent agent;
    public Vector2 objective;
    public List<Vector2> path;


    private bool isInAction = false;

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

    public GameObject GetMostDangerousPredator(List<GameObject> entities)
    {
        GameObject predator = null;
        float highestFear = 0f;

        foreach (GameObject entity in entities)
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

    public List<GameObject> GetNearbyFood(List<GameObject> entities)
    {
        return null;
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
                if (!collider.gameObject.GetComponent<TileManager>().under_water)
                {
                   nearbyTiles.Add(collider.gameObject);
                }
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

    // À compléter
    public GameObject GetFleeingObjective(List<GameObject> entities)
    {
        GameObject predator = GetMostDangerousPredator(entities);
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
    public GameObject GetWaterObjective(List<GameObject> tiles)
    {
        GameObject closestPlacement = null;
        float closestDistance = 0f;
        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<TileManager>().isNearWater)
            {
                foreach (GameObject placement in tile.GetComponent<TileManager>().placementPositions)
                {
                    float distance = Vector3.Distance(transform.position, placement.transform.position);
                    if (placement.GetComponent<PlacementManager>().waterAccess &&
                        (closestPlacement == null || distance < closestDistance))
                    {
                        closestPlacement = placement;
                        closestDistance = distance;
                    }
                }
            }
        }

        if (closestPlacement != null)
        {
            return closestPlacement;
        }

        return null;
    }
    public GameObject GetFoodObjective(List<GameObject> entities)
    {
        GameObject closestFood = null;
        float closestDistance = 0f;
        foreach (GameObject entity in entities)
        {
            if (entity.GetComponent<Entity>().livingEntity is Plant plant)
            {
                if (plant.CanBeEaten())
                {
                    float distance = Vector3.Distance(transform.position, entity.transform.position);

                    if (closestFood == null || distance < closestDistance)
                    {
                        if (data.can_eat.plants.Contains((PlantType)entity.GetComponent<Entity>().type)
                            || data.can_eat.animals.Contains((AnimalType)entity.GetComponent<Entity>().type))
                        {
                            closestFood = entity;
                            closestDistance = Vector3.Distance(transform.position, entity.transform.position);
                        }
                    }
                }
            }
        }

        if (closestFood != null)
        {
            return closestFood;
        }

        return null;
    }
    public GameObject GetMateObjective(List<GameObject> entities)
    {
        // Find a center placement between both entities for mating
        return null;
    }
    public GameObject GetRandomObjective(List<GameObject> tiles)
    {
        GameObject tile = tiles[(int)Random.Range(0, GetNearbyTiles().Count)];
        return tile.GetComponent<TileManager>().centerPlacement;
    }

    public GameObject ChooseObjective()
    {
        List<GameObject> nearbyTiles = GetNearbyTiles();
        List<GameObject> nearbyEntities = GetNearbyEntities();
        
        GameObject fleeingPredatorObjective = GetFleeingObjective(nearbyEntities);
        GameObject waterObjective = GetWaterObjective(nearbyTiles);
        GameObject foodObjective = GetFoodObjective(nearbyEntities);
        GameObject mateObjective = GetMateObjective(nearbyEntities);
        GameObject randomObjective = GetRandomObjective(nearbyTiles);

        // PRIORITY
        // 1: Fleeing
        // 2: Water or Food (smallest value) | If both value greater then 75% of max value, check mating before
        // 3: Mate
        // 4: Water or Food (smallest value)
        // 5: Else: Wander around until next objective

        float matingThreshold = 0.75f; //TODO: Make this variable a part of the animal data
        
        bool isFood = false;
        if (foodObjective != null)
        {
            isFood = true;
        }
        bool isWater = false;
        if (waterObjective != null)
        {
            isWater = true;
        }
        
        bool canMate = false;
        if (hunger < matingThreshold * data.maxHunger && thirst < matingThreshold * data.maxThirst)
        {
            canMate = true;
        }
        
        
        if (fleeingPredatorObjective != null)
        {
            currentObjective = ObjectiveType.Fleeing;
            return fleeingPredatorObjective;
        }
        else if (!canMate && isWater && isFood)
        {
            if (hunger < thirst)
            {
                currentObjective = ObjectiveType.Food;
                return foodObjective;
            }
            else
            {
                currentObjective = ObjectiveType.Water;
                return waterObjective;
            }
        }
        else if (canMate)
        {
            currentObjective = ObjectiveType.Mate;
            return mateObjective;
        }
        else if (isWater)
        {
            currentObjective = ObjectiveType.Water;
            return waterObjective;
        }
        else if (isFood)
        {
            currentObjective = ObjectiveType.Food;
            return foodObjective;
        }
        else
        {
            currentObjective = ObjectiveType.Random;
            return randomObjective;
        }
    }

    public override void SimulationUpdate()
    {
        base.SimulationUpdate();

        GameObject target = ChooseObjective();
        agent.destination = target.transform.position;

        if (HasReachedGoal())
        {
            isInAction = true;

            if (currentObjective == ObjectiveType.Food)
            {
                EatPlant(target);
                Debug.Log("Eating...", gameObject);
            }
            else if (currentObjective == ObjectiveType.Water)
            {
                Debug.Log("Drinking...", gameObject);
            }
            else if (currentObjective == ObjectiveType.Mate)
            {
                Debug.Log("Mating...", gameObject);
            }
            else if (currentObjective == ObjectiveType.Random)
            {
                Debug.Log("Wandering...", gameObject);
            }
        }
        
        hunger -= 0.2f;
        thirst -= 0.2f;
    }

    public void EatPlant(GameObject food)
    {
        if (hunger += food.GetComponent<Plant>().Eat() > data.maxHunger)
        {
            hunger = data.maxHunger;
        }
        else
        {
            hunger += food.GetComponent<Plant>().Eat();
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sensoryDistance);
    }

    private bool HasReachedGoal()
    {
        return !agent.pathPending || agent.remainingDistance < 2f;
    }
}

[System.Serializable]
public class Food
{
    public List<PlantType> plants = new List<PlantType>();
    public List<AnimalType> animals = new List<AnimalType>();
}