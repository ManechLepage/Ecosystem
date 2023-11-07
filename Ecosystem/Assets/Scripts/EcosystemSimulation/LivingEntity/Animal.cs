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
    
    public List<GameObject> GetNearbyTiles(float radius)
    {
        List<GameObject> nearbyTiles = new List<GameObject>();
        
        Vector3 sensoryPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(sensoryPosition, radius);

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

    public List<GameObject> GetNearbyEntities(float radius)
    {
        List<GameObject> nearbyEntities = new List<GameObject>();
        
        Vector3 sensoryPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(sensoryPosition, radius);

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
    public GameObject GetFleeingObjective(List<GameObject> entities, List<GameObject> nearbyTiles)
    {
        GameObject predator = GetMostDangerousPredator(entities);
        if (predator != null)
        {
            // Find the tile that is the furthest from the predator, in range
            GameObject furthestTile = null;

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
        GameObject tile = tiles[(int)Random.Range(0, GetNearbyTiles(sensoryDistance).Count)];
        return tile.GetComponent<TileManager>().centerPlacement;
    }

    public GameObject ChooseObjective()
    {
        List<GameObject> nearbyTiles = GetNearbyTiles(sensoryDistance);
        List<GameObject> nearbyEntities = GetNearbyEntities(sensoryDistance);
        List<GameObject> nearbyWater = GetNearbyEntities(sensoryDistance);
        
        GameObject fleeingPredatorObjective = GetFleeingObjective(nearbyEntities, nearbyTiles);
        GameObject waterObjective = GetWaterObjective(nearbyTiles);
        GameObject foodObjective = GetFoodObjective(nearbyEntities);
        GameObject mateObjective = GetMateObjective(nearbyEntities);
        GameObject randomObjective = GetRandomObjective(nearbyTiles);

        // PRIORITY
        // 1: Fleeing
        // 2: Wander around | If value of water or food is less than 25% and none of this type is found
        // 3: Water or Food (smallest value) | If both value greater then 75% of max value, check mating before
        // 4: Mate
        // 5: Water or Food (smallest value)
        // 6: Wander around

        float matingThreshold = 0.75f;
        float searchingThreshold = 0.50f;
        
        bool isFood = false;
        bool isWater = false;

        bool needsFood = false;
        bool needsWater = false;

        bool canMate = false;

        if (foodObjective != null)
        {
            isFood = true;
        }
        
        if (waterObjective != null)
        {
            isWater = true;
        }

        if (hunger <= searchingThreshold * data.maxHunger)
        {
            needsFood = true;
        }

        if (thirst < searchingThreshold * data.maxThirst)
        {
            needsWater = true;
        }
        
        if (hunger < matingThreshold * data.maxHunger && thirst < matingThreshold * data.maxThirst)
        {
            canMate = true;
        }
        
        
        if (fleeingPredatorObjective != null)
        {
            currentObjective = ObjectiveType.Fleeing;
            return fleeingPredatorObjective;
        }
        else if (needsFood)
        {
            currentObjective = ObjectiveType.Food;
            return foodObjective;
        }
        else if (needsWater)
        {
            currentObjective = ObjectiveType.Water;
            return waterObjective;
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
        if (target != null)
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
                Drink();
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
        float nutrition = food.GetComponent<Plant>().Eat();
        if (hunger + nutrition > data.maxHunger)
        {
            hunger = data.maxHunger;
        }
        else
        {
            hunger += nutrition;
        }
    }

    public void Drink()
    {
        float nutrition = 3f;
        if (thirst + nutrition > data.maxThirst)
        {
            thirst = data.maxThirst;
        }
        else
        {
            thirst += nutrition;
        }
    }
    public bool IsAlive()
    {
        return age < lifespan && hunger > 0f && thirst > 0f;
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