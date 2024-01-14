using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public int gestation_duration;
    public int currentGestation;
    public float hunger;
    public float thirst;
    public int number_of_children;
    public int reproductive_urge;
    public bool isPregnant;
    public GameObject partner;
    public List<GameObject> children = new List<GameObject>();
    public Urge urge_to_run;
    public Urge urge_to_eat;
    public ObjectiveType currentObjective;
    public GameObject currentPrey;
    public bool canReproduce = false;

    [HideInInspector] public float initialHunger = -1f;
    [HideInInspector] public float initialThirst = -1f;
    [HideInInspector] public bool randomlyInitialized = false;
    public bool foundRandomObjective = false;
    public GameObject randomObjective;

    [Header("Navigation")]
    public NavMeshAgent agent;
    public Vector2 objective;
    public List<Vector2> path;


    private bool isInAction = false;

    public bool CanReproduce()
    {
        bool c = reproductive_urge >= data.reproductiveCoolDown && age >= (data.reproductiveMaturity / 365.25f) && !isPregnant;
        canReproduce = c;
        return c;
    }

    public void SetChildren(List<GameObject> _children)
    {
        children = _children;
    }

    public void RandomInitializing()
    {
        age = Random.Range(0f, lifespan * 2f/3f);
        thirst = Random.Range(data.maxThirst / 2f, data.maxThirst);
        hunger = Random.Range(data.maxHunger / 2f, data.maxHunger);
        if (age > data.reproductiveMaturity / 365.25f)
            reproductive_urge = Random.Range(0, (int)((float)data.reproductiveCoolDown * 1.5f));
    }
    public override void Start()
    {
        base.Start();

        lifespan = data.lifespan.get_random_value();
        reproductive_urge = data.reproductiveCoolDown;
        immortal = data.immortal;
        
        sensoryDistance = data.sensory_distance.get_random_value();
        speed = data.speed.get_random_value();
        gestation_duration = (int)Mathf.Round(data.gestation_duration.get_random_value());
        number_of_children = (int)Mathf.Round(data.number_of_children.get_random_value());
        size = data.minMaxSize.x;
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed * 2;

        transform.Rotate(new Vector3(0f, 90f, 0f));
        agent.transform.forward = transform.forward;


        if (initialHunger != -1f)
            hunger = initialHunger;
        else
            hunger = data.maxHunger;

        if (initialThirst != -1f)
            thirst = initialThirst;
        else
            thirst = data.maxThirst;

        urge_to_run = data.urge_to_run.Clone();
        urge_to_eat = data.urge_to_eat.Clone();

        gameObject.transform.position += new Vector3(
            0f,
            gameObject.GetComponent<MeshRenderer>().bounds.size.y / 2,
            0f
        );
    
        currentPrey = null;

        if (randomlyInitialized)
        {
            RandomInitializing();
        }
    }

    public GameObject GetMostDangerousPredator(List<GameObject> entities)
    {
        GameObject predator = null;
        float highestFear = -1f;

        foreach (GameObject entity in entities)
        {
            if (entity.GetComponent<Entity>().type is AnimalType)
            {
                if (((Animal)entity.GetComponent<Entity>().livingEntity).data.can_eat.animals.Contains((AnimalType)gameObject.GetComponent<Entity>().type))
                {
                    System.Enum entityType = (System.Enum)entity.GetComponent<Entity>().type;
                    float urge = urge_to_run.GetValue((AnimalType)entityType);
                    
                    if (urge > highestFear)
                    {
                            predator = entity;
                            highestFear = urge;
                    }
                }
            }
        }
        
        return predator;
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
            if (furthestTile != null)
                return furthestTile.GetComponent<TileManager>().centerPlacement;
            return null;
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
        float highestUrgeToEat = -1f;
        foreach (GameObject entity in entities)
        {
            bool canEatEntity = true;
            if (entity.GetComponent<Entity>().livingEntity is Plant plant)
            {
                if (!plant.CanBeEaten())
                {
                    canEatEntity = false;
                }
            }

            if (canEatEntity)
            {
                float distance = Vector3.Distance(transform.position, entity.transform.position);
                float urge = 1f;
                if (entity.GetComponent<Entity>().type is PlantType)
                    urge = urge_to_eat.GetValue((PlantType)entity.GetComponent<Entity>().type);
                if (entity.GetComponent<Entity>().type is AnimalType)
                    urge = urge_to_eat.GetValue((AnimalType)entity.GetComponent<Entity>().type);

                float random = Random.Range(0f, 1f);
                // multiplier to represent the overall hunger of the animal, so it is more likely to eat anything when it is very hungry
                float multiplier = 1f + (1f - hunger / data.maxHunger) * 2f;
                if (random > urge * multiplier)
                    continue;
                
                if ( closestFood == null || ((
                        highestUrgeToEat == -1f || urge > highestUrgeToEat || (urge == highestUrgeToEat && distance < closestDistance)
                        ))
                    )
                {
                    if (data.can_eat.plants.Contains((PlantType)entity.GetComponent<Entity>().type) &&
                        entity.GetComponent<Entity>().livingEntity is Plant)
                    {
                        closestFood = entity;
                        closestDistance = Vector3.Distance(transform.position, entity.transform.position);
                        highestUrgeToEat = urge;
                    }
                    else if (data.can_eat.animals.Contains((AnimalType)entity.GetComponent<Entity>().type) &&
                        entity.GetComponent<Entity>().livingEntity is Animal)
                    {
                        closestFood = entity;
                        closestDistance = Vector3.Distance(transform.position, entity.transform.position);
                        highestUrgeToEat = urge;
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
        GameObject animal = null;
        if (CanReproduce())
        {
            foreach (GameObject entity in entities)
            {
                // IMPORTANT : mettre le (AnimalType) dans les deux conditions (sinon ça ne marche pas)
                if ((AnimalType)entity.GetComponent<Entity>().type == (AnimalType)gameObject.GetComponent<Entity>().type
                    && entity.GetComponent<Entity>().livingEntity is Animal animalEntity)
                {
                    if (animalEntity.CanReproduce())
                    {
                        animal = entity;
                        break;
                    }
                }
            }
        }

        return animal;
    }
    public GameObject GetRandomObjective()
    {
        if (randomObjective == null)
        {
            int i = 0;
            List<GameObject> tiles = GetNearbyTiles(sensoryDistance * 5);
            if (tiles.Count == 0)
            {
                return null;
            }
            GameObject tile = tiles[(int)Random.Range(0, tiles.Count)];
            while (tile.GetComponent<TileManager>().isBorder)
            {
                tile = tiles[(int)Random.Range(0, tiles.Count)];
                i++;
                if (i > 100)
                {
                    break;
                }
            }
            randomObjective = tile.GetComponent<TileManager>().centerPlacement;
            return randomObjective;
        }
        return randomObjective;
    }

    public GameObject ChooseObjective()
    {
        List<GameObject> nearbyTiles = GetNearbyTiles(sensoryDistance);
        List<GameObject> nearbyEntities = GetNearbyEntities(sensoryDistance);
        List<GameObject> nearbyWater = GetNearbyEntities(sensoryDistance);
        nearbyEntities.Remove(gameObject);
        
        GameObject fleeingPredatorObjective = GetFleeingObjective(nearbyEntities, nearbyTiles);
        GameObject waterObjective = GetWaterObjective(nearbyTiles);
        GameObject foodObjective = GetFoodObjective(nearbyEntities);
        GameObject mateObjective = GetMateObjective(nearbyEntities);
        GameObject randomObjective = GetRandomObjective();

        // PRIORITY
        // 1: Fleeing
        // 2: Wander around | If value of water or food is less than 50% and none of this type is found
        // 3: Water or Food (smallest value) | If both value greater then 75% of max value, check mating
        // 4: Mate
        // 5: Water or Food (smallest value)
        // 6: Wander around

        float eatingAndDrinkingThreshold = 0.85f;
        float matingThreshold = 0.75f;
        float searchingThreshold = 0.50f;
        
        bool isFood = false;
        bool isWater = false;

        bool needsUrgentFood = false;
        bool needsUrgentWater = false;

        bool needsFood = false;
        bool needsWater = false;

        bool needsSomeFood = false;
        bool needsSomeWater = false;

        bool canMate = false;
        bool isMate = false;

        if (foodObjective != null)
            isFood = true;
        if (waterObjective != null)
            isWater = true;

        if (hunger <= searchingThreshold * data.maxHunger)
            needsUrgentFood = true;
        if (thirst < searchingThreshold * data.maxThirst)
            needsUrgentWater = true;
        if (hunger < matingThreshold * data.maxHunger)
            needsFood = true;
        if (thirst < matingThreshold * data.maxThirst)
            needsWater = true;
        if (hunger < eatingAndDrinkingThreshold * data.maxHunger)
            needsSomeFood = true;
        if (thirst < eatingAndDrinkingThreshold * data.maxThirst)
            needsSomeWater = true;
        
        if (CanReproduce())
            canMate = true;
        if (mateObjective != null)
            isMate = true;
        
        // 1: Fleeing
        if (fleeingPredatorObjective != null)
        {
            currentObjective = ObjectiveType.Fleeing;
            return fleeingPredatorObjective;
        }

        // 2: Wander around | If value of water or food is less than 25% and none of this type is found
        else if (
            (needsUrgentFood && !isFood) ||
            (needsUrgentWater && !isWater)
            )
        {
            currentObjective = ObjectiveType.Random;
            return randomObjective;
        }

        // 3: Water or Food (smallest value) | If both value greater then 75% of max value, check mating
        else if ((needsUrgentFood || needsFood)
            && isFood && (hunger < thirst || !isWater ||
            (!needsWater && !needsUrgentWater)))
        {
            currentObjective = ObjectiveType.Food;
            return foodObjective;
        }
        else if ((needsUrgentWater || needsWater) && isWater)
        {
            currentObjective = ObjectiveType.Water;
            return waterObjective;
        }

        // 4: Mate
        else if (canMate && isMate)
        {
            currentObjective = ObjectiveType.Mate;
            return mateObjective;
        }

        // 5: Water or Food (smallest value) if below 85% of max value
        else if (isFood && needsSomeFood && (
            hunger < thirst || !isWater || !needsSomeWater))
        {
            currentObjective = ObjectiveType.Food;
            return foodObjective;
        }
        else if (isWater && needsSomeWater)
        {
            currentObjective = ObjectiveType.Water;
            return waterObjective;
        }

        // 6: Wander around
        else
        {
            currentObjective = ObjectiveType.Random;
            return randomObjective;
        }
    }

    public override void SimulationUpdate(int days)
    {
        base.SimulationUpdate(days);

        GameObject target = ChooseObjective();
        if (target != null)
            agent.destination = target.transform.position;

            if (currentObjective == ObjectiveType.Food)
                currentPrey = target;
            else
                currentPrey = null;
            
            if (currentObjective == ObjectiveType.Mate)
                partner = target;
            else
                partner = null;


        if (HasReachedGoal())
        {
            isInAction = true;

            if (currentObjective == ObjectiveType.Food && currentPrey != null &&
                currentPrey.GetComponent<Entity>().livingEntity is Plant)
            {
                EatPlant(currentPrey);
            }
            else if (currentObjective == ObjectiveType.Water)
            {
                Drink();
            }
            else if (currentObjective == ObjectiveType.Random)
            {
                randomObjective = null;
            }
        }
        
        hunger -= 0.2f;
        thirst -= 0.2f;
        reproductive_urge += days;

        if (isPregnant)
        {
            currentGestation += days;
        }

        if (currentGestation >= gestation_duration)
        {
            isPregnant = false;
            currentGestation = 0;
            GiveBirth();
        }
    }

    public void Update()
    {
        if (currentObjective == ObjectiveType.Food && currentPrey != null)
        {
            agent.destination = currentPrey.transform.position;
        }

        if (currentObjective == ObjectiveType.Mate && partner != null)
        {
            agent.destination = partner.transform.position;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == currentPrey && currentObjective == ObjectiveType.Food)
        {
            isInAction = false;
            currentPrey = null;
            if (other.gameObject.GetComponent<Entity>().livingEntity is Plant plant)
                EatPlant(other.gameObject);
            else
            {
                EatAnimal(other.gameObject);
                Debug.Log(gameObject.name + " ate " + other.gameObject.name, gameObject);
            }
        }

        if (other.gameObject == partner && currentObjective == ObjectiveType.Mate)
        {
            isInAction = false;
            partner = null;
            Mate(other.gameObject);
        }
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

    public void EatAnimal(GameObject food)
    {
        float nutrition = food.GetComponent<Animal>().Eat();
        if (hunger + nutrition > data.maxHunger)
        {
            hunger = data.maxHunger;
        }
        else
        {
            hunger += nutrition;
        }
currentPrey = null;
    }

    public float Eat()
    {
        Die();
        return data.nutrition; 
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

    public void Mate(GameObject animalToMate)
    {
        reproductive_urge = 0;
        currentGestation = 0;
        
        SimulationManager simulationManager = GameObject.Find("Simulator").GetComponent<SimulationManager>();
        simulationManager.AddAnimalToMating(gameObject, animalToMate);
    }

    public void GiveBirth()
    {
        SimulationManager simulationManager = GameObject.Find("Simulator").GetComponent<SimulationManager>();
        simulationManager.Reproduce(gameObject, partner);
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

    public void FoundObjective()
    {
        foundRandomObjective = true;
    }

    private bool HasReachedGoal()
    {
        return agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }

    public void Die()
    {
        age = lifespan;
    }
}

[System.Serializable]
public class Food
{
    public List<PlantType> plants = new List<PlantType>();
    public List<AnimalType> animals = new List<AnimalType>();
}