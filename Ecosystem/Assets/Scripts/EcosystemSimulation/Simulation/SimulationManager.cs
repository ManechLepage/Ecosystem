using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Unity.AI.Navigation;

public enum TileType
{
    Grass,
    Sand,
    Rock
}

public enum BiomeType
{
    Plains,
    Forest,
    Desert,
    Mountain
}

[System.Serializable]
public class AnimalPrefab
{
    public AnimalType type;
    public GameObject prefab;
}

[System.Serializable]
public class PlantPrefab
{
    public PlantType type;
    public GameObject prefab;
}


public class SimulationManager : MonoBehaviour
{
    [Header("General Settings")]
    public int seed = -1; // If -1, the seed is random
    public Vector2 size = new Vector2(64, 64);
    public EcosystemData ecosystemData;
    [HideInInspector] public List<Event> executedEvents = new List<Event>();
    public GameObject camera;
    public bool pauseWhenNoAnimals = true;

    [Header("Terrain Settings")]
    public float tile_size = 1f;
    public float definition_quality = 5f;
    public GameObject terrainParent;

    [Header("Entities Settings")]
    public GameObject entitiesParent;
    private Dictionary<GameObject, GameObject> allMatingAnimals = new Dictionary<GameObject, GameObject>();

    [Header("Water Settings")]
    public GameObject waterPlane;

    [Header("Tile Materials Prefabs")]
    public Material grassTile;
    public Material sandTile;
    public Material rockTile;
    public Material dirtTile;
    [Space]
    public GameObject tilePrefab;

    [Header("Entity Prefabs")]
    public AnimalPrefab[] animalPrefabs;
    public PlantPrefab[] plantPrefabs;
    
    private Dictionary<TileType, List<Material>> tileMaterials;

    [Header("Other Settings")]
    public float time = 0f;
    public int simulationDays = 0;
    public int daysPerUpdate = 4;
    public List<List<GameObject>> tiles;

    public Dictionary<System.Enum, int> populations = new Dictionary<System.Enum, int>() {};
    public bool add_walls = true;

    [Space]
    public List<GameObject> entities = new List<GameObject>();
    public GameObject water_plane;
    [Space]
    public NavMeshSurface surface;

    private int simulationAge = 0;
    [HideInInspector] public System.Random randomWithSeed;
    public bool pause = false;
    
    public void Initialize()
    {
        randomWithSeed = new System.Random(seed);
        tiles = new List<List<GameObject>>();
        entities = new List<GameObject>();
        populations = new Dictionary<System.Enum, int>() {};

        tileMaterials = new Dictionary<TileType, List<Material>>() {
            {TileType.Grass, new List<Material>() {dirtTile, grassTile, dirtTile}}, // Bottom, Top, Transition (middle)
            {TileType.Sand, new List<Material>() {dirtTile, sandTile, sandTile}},
            {TileType.Rock, new List<Material>() {rockTile, grassTile, dirtTile}}
        }; 
        
        if (seed == -1)
        {
            seed = Random.Range(1, 100_000); // DO NOT make this number bigger, it will cause terrain generation bugs
        }
    }
    
    public void Start()
    {
        Initialize();
        GenerateEcosystem();
    }

    public void AddWater()
    {
        water_plane = Instantiate(waterPlane);
        water_plane.transform.parent = terrainParent.transform;

        // Because the grid is composed of hexagons, the size of the plane is not the same as the size of the grid

        float x_size_factor = Mathf.Sqrt(3f);
        float y_size_factor = Mathf.Sqrt(2f) * Mathf.Sqrt(Mathf.Sqrt(1.25f));

        water_plane.transform.localScale = new Vector3(
            size.x * tile_size * definition_quality / 10 * x_size_factor,
            1,
            size.y * tile_size * definition_quality / 10 * y_size_factor);

        float y_pos = ecosystemData.waterLevel;
        
        water_plane.transform.position = new Vector3(
            water_plane.transform.localScale.x * 10 / 2,
            Mathf.Round(y_pos * 20f) * definition_quality + 1f,
            water_plane.transform.localScale.z * 10 / 2);
    }

    GameObject AddLivingEntity(GameObject placement, System.Enum type, bool randomInitializing=true) // Change to give a placement position and not a tile
    {
        GameObject prefab = GetGameObjectFromType(type);

        if (prefab != null && placement != null)
        {
            GameObject go = GameObject.Instantiate(
                prefab,
                placement.transform.position,
                Quaternion.identity
            );
            go.transform.parent = entitiesParent.transform;
            go.transform.localScale = new Vector3(
                go.transform.localScale.x,
                go.transform.localScale.y,
                go.transform.localScale.z
            );

            Vector3 position = placement.transform.position;

            go.transform.position = position;
            GameObject spawning_tile = null;
            int spawnEmptyIndex = -1;
            
            if (placement.GetComponent<PlacementManager>() != null)
            {
                spawning_tile = placement.GetComponent<PlacementManager>().tileParent;
                spawnEmptyIndex = System.Array.IndexOf(
                    spawning_tile.GetComponent<TileManager>().placementPositions,
                    placement
                );

                if (placement == spawning_tile.GetComponent<TileManager>().centerPlacement)
                    spawning_tile.GetComponent<TileManager>().centerPopulation = go;
                else
                {
                    spawning_tile.GetComponent<TileManager>().tilePopulation[spawnEmptyIndex] = go;
                }
            }

            go.GetComponent<Entity>().type = type;
            go.GetComponent<Entity>().livingEntity = GetLivingEntityFromEntity(type, go);

            if (type is PlantType && spawning_tile != null && spawnEmptyIndex != null)
            {
                go.transform.parent = spawning_tile.transform;
                spawning_tile.GetComponent<TileManager>().Populate(spawnEmptyIndex, go);
            }

            go.GetComponent<Entity>().livingEntity.Start();
            if (go.GetComponent<Entity>().livingEntity is Animal && randomInitializing)
                ((Animal)go.GetComponent<Entity>().livingEntity).randomlyInitialized = true;

            AddEntitiesToPopulations(type);
            entities.Add(go);
            return go;
        }

        return null;
    }

    public LivingEntity GetLivingEntityFromEntity(System.Enum type, GameObject entity)
    {
        LivingEntity livingEntity = null;
        if (type is AnimalType)
            livingEntity = entity.GetComponent<Animal>();
        else if (type is PlantType)
            livingEntity = entity.GetComponent<Plant>();
        return livingEntity;
    }

    void AddEntitiesToPopulations(System.Enum type, int count=1)
    {
        if (populations.ContainsKey(type))
        {
            populations[type] += count;
        }
        else
        {
            populations[type] = count;
        }

        if (populations[type] < 0)
        {
            populations.Remove(type);
        }
    }

    GameObject GetGameObjectFromType(System.Enum type)
    {
        if (type.GetType() == typeof(AnimalType))
        {
            foreach (AnimalPrefab animal_prefab in animalPrefabs)
            {
                if (animal_prefab.type == (AnimalType)type)
                {
                    return animal_prefab.prefab;
                }
            }
        }
        else if (type.GetType() == typeof(PlantType))
        {
            foreach (PlantPrefab plant_prefab in plantPrefabs)
            {
                if (plant_prefab.type == (PlantType)type)
                {
                    return plant_prefab.prefab;
                }
            }
        }

        return null;
    }

    public TileType get_type(Vector2 position)  // Add height as an argument
    {
        float dist = distance_from_side(position);

        return dist < 0.3f? TileType.Grass : TileType.Rock;
    }

    public void PopulateTerrain()
    {
        // Add plants
        for (int x = 2; x < tiles.Count - 3; x++)
        {
            for (int y = 2; y < tiles[x].Count - 3; y++)
            {
                // If the top of the tile is under water, skip it
                if (tiles[x][y].GetComponent<TileManager>().centerPlacement.transform.position.y
                    <= water_plane.transform.position.y + 1f) // 1f is for the animation of water
                {
                    tiles[x][y].GetComponent<TileManager>().under_water = true;
                    continue;
                }
                
                float noise_value_1 = Mathf.PerlinNoise(x / 100 + seed * 7, y / 100 + seed * 7);
                float noise_value_2 = Mathf.PerlinNoise(x / 10 + seed * 14, y / 10 + seed * 14);

                float noise_value = 0.5f + (noise_value_1 * 0.75f + noise_value_2 * 0.25f) / 2f / 2f;
                int probability = (int)Mathf.Max(Mathf.Round(noise_value * 15f * (1 / ecosystemData.plantDensity)), 1);
                
                for (int i = 0; i < 6; i++)
                {
                    if (tiles[x][y].GetComponent<TileManager>().type == TileType.Grass && randomWithSeed.Next(0, probability + 1) <= 1)
                    {
                        PlantType plant_type = ecosystemData.populations.GetRandomPlant(randomWithSeed);
                        
                        AddLivingEntity(
                            tiles[x][y].GetComponent<TileManager>().placementPositions[i],
                            (System.Enum)plant_type
                        );
                    }
                }
            }
        }

        // Add animals
        for (int x = 0; x < Mathf.Round(tiles.Count * tiles[0].Count / 25 * ecosystemData.animalDensity); x++)
        {
            System.Enum animal_type = ecosystemData.populations.GetRandomAnimal(randomWithSeed);
            AddAnimalToRandomPosition(animal_type);
        }

        string pop_text = "Populations :\n";
        foreach (KeyValuePair<System.Enum, int> population in populations)
        {
            pop_text += "   " + population.Key.ToString() + " : " + population.Value.ToString() + "\n";
        }

        Debug.Log(pop_text);

        foreach (GameObject entity in entities)
        {
            if (entity.GetComponent<Entity>().type is AnimalType)
            {
                camera.transform.position = new Vector3(0f, 0f, 0f);
                camera.transform.SetParent(entity.transform);
                Debug.Log("Camera parent set", entity);
                break;
            }
        }
        
    }

    public void AddAnimalToRandomPosition(System.Enum type)
    {
        GameObject tile = null;
        TileType tile_type = TileType.Rock;  // IMPORTANT : set to any type that is not grass
        int count = 0;
        
        while (tile == null || tile_type == null || tile_type != TileType.Grass || tile.GetComponent<TileManager>().under_water == true)
        {
            int random_x = randomWithSeed.Next(3, tiles.Count - 4);
            int random_y = randomWithSeed.Next(3, tiles[random_x].Count - 4);

            tile = tiles[random_x][random_y];
            tile_type = tile.GetComponent<TileManager>().type;
            count++;

            if (count > 100)
            {
                break;
            }
        }

        // in case of no tile found
        if (tile_type == TileType.Grass && tile.GetComponent<TileManager>().under_water == false)
        {
            AddLivingEntity(tile.GetComponent<TileManager>().centerPlacement, type);
        }
    }
    
    public void Awake()
    {
        // surface.BuildNavMesh();
        StartCoroutine(SimulationLoop());
    }

    public void AddAnimalToMating(GameObject animal, GameObject animalToMate)
    {
        allMatingAnimals.Add(animal, animalToMate);
    }

    public void SimulationUpdate()
    {
        simulationDays += daysPerUpdate;

        foreach (Event e in ecosystemData.events)
        {
            if (e.day <= simulationDays && !executedEvents.Contains(e))
            {
                for (int i = 0; i < e.count; i++)
                {
                    AddAnimalToRandomPosition((System.Enum)e.animalType);
                }
                executedEvents.Add(e);
            }
        }
        
        List<GameObject> deadEntities = new List<GameObject>();
        // To prevent the childs of getting modified while iterating
        List<GameObject> all_entities = new List<GameObject>(entities);
        foreach (GameObject living_entity in all_entities)
        {
            if (living_entity.GetComponent<Entity>().livingEntity != null)
            {
                living_entity.GetComponent<Entity>().livingEntity.SimulationUpdate(daysPerUpdate);

                if (living_entity.GetComponent<Entity>().livingEntity is Animal animalEntity)
                {
                    if (!animalEntity.IsAlive())
                    {
                        deadEntities.Add(living_entity);
                    }
                }
            }
        }

        List<GameObject> alreadyMated = new List<GameObject>();
        foreach (KeyValuePair<GameObject, GameObject> animal in allMatingAnimals)
        {
            if (!alreadyMated.Contains(animal.Key) && !alreadyMated.Contains(animal.Value))
            {
                Animal animal1 = (Animal)animal.Key.GetComponent<Entity>().livingEntity;
                Animal animal2 = (Animal)animal.Value.GetComponent<Entity>().livingEntity;

                animal1.isPregnant = true;
                animal1.partner = animal.Value;

                alreadyMated.Add(animal.Key);
                alreadyMated.Add(animal.Value);
            }
        }
        allMatingAnimals.Clear();

        foreach (GameObject deadEntity in deadEntities)
        {
            entities.Remove(deadEntity);
            Destroy(deadEntity);
        }

        // pause if no animals left -> look in the entitiesParent children (if none, pause)
        bool noAnimalsLeft = true;
        foreach (Transform child in entitiesParent.transform)
        {
            noAnimalsLeft = false;
            break;
        }

        if (noAnimalsLeft)
        {
            Debug.Log("No animals left, pausing simulation");
            pause = true;
        }
    }

    public void Reproduce(GameObject animal1, GameObject animal2)
    {
        Animal animal1Entity = (Animal)animal1.GetComponent<Entity>().livingEntity;
        // TODO: store the infos of the father somewere in case of him dying

        List<GameObject> children = new List<GameObject>();

        GameObject empty = new GameObject();
        // randomize the position (-0.005 to 0.005 for each axis)
        empty.transform.position = new Vector3(
            animal1.transform.position.x + Random.Range(-0.005f, 0.005f),
            animal1.transform.position.y + Random.Range(-0.005f, 0.005f),
            animal1.transform.position.z + Random.Range(-0.005f, 0.005f)
        );
        empty.transform.rotation = animal1.transform.rotation;
        
        for (int i = 0; i < animal1Entity.number_of_children; i++)
        {
            GameObject child = AddLivingEntity(
                empty,
                animal1.GetComponent<Entity>().type,
                randomInitializing: false
            );
            // fix the hunger and thirst of the child -> do not set...
            if (child != null)
            {
                Animal childAnimal = (Animal)child.GetComponent<Entity>().livingEntity;
                childAnimal.initialHunger = animal1Entity.hunger;
                childAnimal.initialThirst = animal1Entity.thirst;
                children.Add(child);
            }
        }
        DestroyImmediate(empty);

        animal1Entity.SetChildren(children);
        animal1Entity.number_of_children = (int)Mathf.Round(animal1Entity.data.number_of_children.get_random_value());
        Debug.Log("Children set", animal1);
    }
    
    public IEnumerator SimulationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (pause)
                continue;
            SimulationUpdate();
            Debug.Log("Simulation updated");
            simulationAge++;
            CreateAnimalData("Rabbit");
            CreateAnimalData("Fox");
        }
    }

    public void CreateAnimalData(string animalName)
    {
        List<float> age = new List<float>();
        List<float> speed = new List<float>();
        List<float> thirst = new List<float>();
        List<float> hunger = new List<float>();

        foreach (GameObject entity in entities)
        {
            LivingEntity livingEntity = entity.GetComponent<LivingEntity>();
            int counter = 0;

            if (livingEntity is Animal animalEntity && livingEntity.GetType().Name == animalName)
            {
                age.Add(animalEntity.age);
                speed.Add(animalEntity.speed);

                // Normalize thirst and hunger to make it universal
                // thirst.Add(animalEntity.thirst / animalEntity.data.maxThirst);
                // hunger.Add(animalEntity.hunger / animalEntity.data.maxHunger);
                thirst.Add(animalEntity.thirst);
                hunger.Add(animalEntity.hunger);
                counter ++;
            }
        }

        gameObject.GetComponent<SendInfoToSheets>().SendAnimalData(
            simulationAge, animalName,
            GetAverageFromList(age),
            GetAverageFromList(speed),
            GetAverageFromList(thirst),
            GetAverageFromList(hunger)//,
            //counter
        );
    }

    float GetAverageFromList(List<float> list)
    {
        float sum = 0f;
        foreach (float value in list)
        {
            sum += value;
        }

        return sum / list.Count;
    }

    public void GenerateTerrain()
    {
        AddWater();
        
        float x_pos = 0;
        float y_pos = 0;

        for (int x = 0; x < size.x; x++)
        {
            List<GameObject> column = new List<GameObject>();
            for (int y = 0; y < size.y; y++)
            {
                float offset = (x % 2 == 0 ? Mathf.Sqrt(0.75f) : 0f);

                TileType type = get_type(new Vector2(x, y));
                List<Material> tileMats = tileMaterials[type];

                GameObject tile = GameObject.Instantiate(tilePrefab);
                tile.transform.parent = terrainParent.transform;

                tile.GetComponent<MeshRenderer>().sharedMaterials = tileMats.ToArray();

                TileManager tileInfo = tile.GetComponent<TileManager>();
                tileInfo.position = new Vector2(x_pos + offset, y_pos);
                tileInfo.height = Mathf.Round(get_real_height(new Vector2(x, y)));

                tile.transform.position = new Vector3(
                        tileInfo.position.x * definition_quality * tile_size,
                        tileInfo.height * definition_quality,
                        tileInfo.position.y * definition_quality * tile_size);

                tile.transform.localScale = new Vector3(
                    tile.transform.localScale.x * tile_size * definition_quality,
                    tile.transform.localScale.y * tile_size * definition_quality,
                    tile.transform.localScale.z * tile_size * definition_quality * 0.2f);

                foreach (GameObject placement in tile.GetComponent<TileManager>().placementPositions)
                {
                    placement.GetComponent<PlacementManager>().tileParent = tile;
                }

                column.Add(tile);
                x_pos += Mathf.Sqrt(3f);
            }
            tiles.Add(column);
            y_pos += 0.75f * 2f;
            x_pos = 0;
        }
    }

    public void GenerateEcosystem()
    {
        GenerateTerrain();
        surface.BuildNavMesh();
        
        PopulateTerrain();
        
        surface.BuildNavMesh();
    }

    public void DeleteTerrain()
    {
        for (int x = 0; x < tiles.Count; x++)
        {
            for (int y = 0; y < tiles[x].Count; y++)
            {
                DestroyImmediate(tiles[x][y]);
            }
            tiles[x].Clear();
        }
        tiles.Clear();

        foreach (GameObject living_entity in entities)
        {
            DestroyImmediate(living_entity);
        }

        DestroyImmediate(water_plane);

        surface.RemoveData();
    }

    private float blocks_distance_from_side(Vector2 position)
    {
        float distance_from_side_x = Mathf.Min(position.x, size.x - position.x - 1);
        float distance_from_side_y = Mathf.Min(position.y, size.y - position.y - 1);

        return Mathf.Min(distance_from_side_x, distance_from_side_y);
    }
    
    // The two next functions are used to create walls around the map
    
    private float distance_from_side(Vector2 position)
    {
        float diff_seed = seed * 5f;
        float distance_from_side = blocks_distance_from_side(position);

        float final_value = Mathf.Pow(Mathf.Max(1f - distance_from_side / 6, 0f), 3.5f);
        final_value *=  1 + (Mathf.PerlinNoise(position.x / 5f + position.y / 5f + diff_seed, final_value + diff_seed) / 4f - 0.1f);

        return final_value;
    }

    public float get_height(Vector2 position)
    {
        float diff_seed = seed * 10f;
        /*float smoothness = 1f;
        float intensity = 1f;

        if (biome == BiomeType.Plains)
        {
            smoothness = 1f;
            intensity = 1f;
        }
        else if (biome == BiomeType.Forest)
        {
            smoothness = 1.5f;
            intensity = 3f;
        }*/
        
        // two random noises combined together
        float first_noise_value = Mathf.PerlinNoise(
            position.x / (15f * ecosystemData.smoothness) + diff_seed,
            position.y / (15f * ecosystemData.smoothness) + diff_seed
        );
        float second_noise_value = Mathf.PerlinNoise(
            position.x / (8f * ecosystemData.smoothness) + diff_seed,
            position.y / (8f * ecosystemData.smoothness) + diff_seed
        );

        return (first_noise_value + second_noise_value) / 2f * 12f * ecosystemData.intensity;
    }

    private float get_real_height(Vector2 position)
    {
        // Here we want to use distance_from_side to modify the height of the tile, to create walls around the map
        // To do this, the distance from side value will be modified so the walls get a high angle (using a power function)

        float distance_from_side_value = 0f;
        if (add_walls)
        {
            distance_from_side_value = distance_from_side(position);
        } 

        return get_height(position) + distance_from_side_value * 35f;
    } 

    public void OnApplicationQuit()
    {
        DeleteTerrain();
    }
}