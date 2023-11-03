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
    public float plantsDensity = 1f;
    public float animalsDensity = 1f;

    [Header("Terrain Settings")]
    public float tile_size = 1f;
    public float definition_quality = 5f;
    public GameObject terrainParent;

    [Header("Entities Settings")]
    public GameObject entitiesParent;

    [Header("Water Settings")]
    [SerializeField] public GameObject waterPlane;

    [Header("Tile Materials Prefabs")]
    [SerializeField] public Material grassTile;
    [SerializeField] public Material sandTile;
    [SerializeField] public Material rockTile;
    [SerializeField] public Material dirtTile;
    [Space]
    public GameObject tilePrefab;

    [Header("Entity Prefabs")]
    public AnimalPrefab[] animalPrefabs;
    public PlantPrefab[] plantPrefabs;
    
    private Dictionary<TileType, List<Material>> tileMaterials;

    [Header("Biome Settings")]
    public float time;
    public List<List<GameObject>> tiles;

    public BiomeType biome;
    public Dictionary<System.Enum, int> populations = new Dictionary<System.Enum, int>() {};
    public bool add_walls = true;

    [Space]
    public List<GameObject> entities = new List<GameObject>();
    public GameObject water_plane;
    [Space]
    public NavMeshSurface surface;
    
    public void Initialize()
    {
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

        float y_pos = 1f;

        if (biome == BiomeType.Plains)
            y_pos = 0.3f;
        else if (biome == BiomeType.Forest)
            y_pos = 0.7f;
        
        water_plane.transform.position = new Vector3(
            water_plane.transform.localScale.x * 10 / 2,
            Mathf.Round(y_pos * 20f) * definition_quality + 1f,
            water_plane.transform.localScale.z * 10 / 2);
    }

    void AddLivingEntity(GameObject spawning_tile, int spawnEmptyIndex, System.Enum type)
    {
        GameObject prefab = GetGameObjectFromType(type);
        GameObject spawning_tile_empty;

        if (spawnEmptyIndex == -1)
        {
            spawning_tile_empty = spawning_tile.GetComponent<TileManager>().centerPlacement;
        }
        else
        {
            spawning_tile_empty = spawning_tile.GetComponent<TileManager>().placementPositions[spawnEmptyIndex];
        }

        if (prefab != null)
        {
            GameObject go = GameObject.Instantiate(prefab);
            go.transform.parent = entitiesParent.transform;
            go.transform.localScale = new Vector3(
                go.transform.localScale.x,
                go.transform.localScale.y,
                go.transform.localScale.z
            );

            Vector3 position = spawning_tile_empty.transform.position;

            go.transform.position = position;
            
            if (spawning_tile_empty == spawning_tile.GetComponent<TileManager>().centerPlacement)
                spawning_tile.GetComponent<TileManager>().centerPopulation = go;
            else
            {
                int empty_index = System.Array.IndexOf(spawning_tile.GetComponent<TileManager>().placementPositions, spawning_tile_empty);
                spawning_tile.GetComponent<TileManager>().tilePopulation[empty_index] = go;
            }

            // changer le code pour la prochaine partie (très moche)
            go.GetComponent<LivingEntityType>().type = type;
            LivingEntity livingEntity = GetLivingEntityFromEntity(type, go);

            if (type is PlantType) 
            {
                go.transform.parent = spawning_tile.transform;
                spawning_tile.GetComponent<TileManager>().Populate(spawnEmptyIndex, go); 
            }

            livingEntity.Start();
            AddEntitiesToPopulations(type);
            entities.Add(go);
        }
    }

    LivingEntity GetLivingEntityFromEntity(System.Enum type, GameObject entity)
    {
        LivingEntity livingEntity = null;
        switch (type)
        {
            case AnimalType.rabbit:
                livingEntity = entity.GetComponent<Rabbit>();
                break;
            case PlantType.herb:
                livingEntity = entity.GetComponent<Herb>();
                break;
            case PlantType.oakTree:
                livingEntity = entity.GetComponent<OakTree>();
                break;
        }

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
        var populations_random = new System.Random(seed);
        
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
                int probability = (int)Mathf.Max(Mathf.Round(noise_value * 15f * (1 / plantsDensity)), 1);
                
                for (int i = 0; i < 6; i++)
                {
                    if (tiles[x][y].GetComponent<TileManager>().type == TileType.Grass && populations_random.Next(0, probability + 1) <= 1)
                    {
                        System.Enum plant_type = PlantType.herb;

                        if (populations_random.Next(0, 20) <= 1)
                        {
                            plant_type = PlantType.oakTree;
                        }
                        
                        AddLivingEntity(tiles[x][y], i, plant_type);
                    }
                }

            }
        }

        // Add animals

        for (int x = 0; x < Mathf.Round(tiles.Count * tiles[0].Count / 25 * animalsDensity); x++)
        {
            int random_x = populations_random.Next(3, tiles.Count - 4);
            int random_y = populations_random.Next(3, tiles[random_x].Count - 4);

            GameObject tile = tiles[random_x][random_y];
            TileType type = tile.GetComponent<TileManager>().type;

            if (type == TileType.Grass && tile.GetComponent<TileManager>().under_water == false)
            {
                AddLivingEntity(tile, -1, (System.Enum)AnimalType.rabbit);
            }
        }
    }

    public void Awake()
    {
        // surface.BuildNavMesh();
        StartCoroutine(SimulationLoop());
    }

    public void SimulationUpdate()
    {
        foreach (GameObject living_entity in entities)
        {
            LivingEntity livingEntity = GetLivingEntityFromEntity(living_entity.GetComponent<LivingEntityType>().type, living_entity);
            if (livingEntity != null)
            {
                livingEntity.SimulationUpdate();
            }
        }
    }
    
    public IEnumerator SimulationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            SimulationUpdate();
            Debug.Log("Simulation updated");
        }
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
        float smoothness = 1f;
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
        }
        
        // two random noises combined together
        float first_noise_value = Mathf.PerlinNoise(position.x / (15f * smoothness) + diff_seed, position.y / (15f * smoothness) + diff_seed);
        float second_noise_value = Mathf.PerlinNoise(position.x / (8f * smoothness) + diff_seed, position.y / (8f * smoothness) + diff_seed);

        return (first_noise_value + second_noise_value) / 2f * 12f * intensity;
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


    void AddTile(Vector2 position, float height) // C'est nécessaire ?
    {
        // TODO
    }

    void Update()
    {
        // foreach (GameObject living_entity in livingEntityConversion)
        // {
        //     if (living_entity.GetComponent<PlantBehaviour>() != null)
        //     {
        //         PlantBehaviour plant = living_entity.GetComponent<PlantBehaviour>();
        //         plant.simulated_living.update(Time.deltaTime);
        //     }
        //     else if (living_entity.GetComponent<AnimalBehaviour>() != null)
        //     {
        //         AnimalBehaviour animal = living_entity.GetComponent<AnimalBehaviour>();
        //         animal.simulated_living.update(Time.deltaTime);
        //     }
        // }
    }

    public void OnApplicationQuit()
    {
        DeleteTerrain();
    }
}
