using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum TileType
{
    Grass,
    Sand,
    Rock
}

public class SimulationManager : MonoBehaviour
{
    [Header("General Settings")]
    public int seed = -1; // If -1, the seed is random
    public Vector2 size = new Vector2(64, 64);

    [Header("Terrain Settings")]
    public float tile_size = 1f;
    public float definition_quality = 5f;
    public GameObject terrainParent;

    [Header("Tile Materials Prefabs")]
    [SerializeField] public Material grassTile;
    [SerializeField] public Material sandTile;
    [SerializeField] public Material rockTile;
    [SerializeField] public Material dirtTile;
    [Space]
    public GameObject tilePrefab;

    [Header("Animal Prefabs")]
    [SerializeField] public GameObject rabbitPrefab;

    [Header("Plant Prefabs")]
    [SerializeField] public GameObject herbPrefab;

    [Header("Living Things Materials")]
    [SerializeField] public Material herbMaterial;

    private Dictionary<TileType, List<Material>> tileMaterials;

    [Header("Biome Settings")]
    public float time;
    public List<List<GameObject>> tiles;

    public string biomeName;
    public Dictionary<System.Type, int> populations = new Dictionary<System.Type, int>() {};
    public bool add_walls = true;

    [Space]
    public List<GameObject> living_things_list;
    
    public void Start()
    {

        tiles = new List<List<GameObject>>();

        tileMaterials = new Dictionary<TileType, List<Material>>() {
            {TileType.Grass, new List<Material>() {dirtTile, grassTile, dirtTile}}, // Bottom, Top, Transition (middle)
            {TileType.Sand, new List<Material>() {dirtTile, sandTile, sandTile}},
            {TileType.Rock, new List<Material>() {rockTile, grassTile, dirtTile}}
        }; 
        
        if (seed == -1)
        {
            seed = Random.Range(1, 100_000); // DO NOT make this number bigger, it will cause terrain generation bugs
        }

        // Temporary : add a rabbit
        // GameObject rabbit_go = GameObject.Find("Rabbit");
        // rabbit_go.GetComponent<AnimalBehaviour>().SetSimulation(gameObject.GetComponent<SimulationManager>());
        // rabbit_go.GetComponent<AnimalBehaviour>().Initialize();

    }

    void AddAnimal(GameObject spawning_tile, System.Type type)
    {
        // If the type is a key of populations, add 1 to it, else add it to the dictionary and set it to 1
        if (populations.ContainsKey(type))
            populations[type] += 1;
        else
            populations.Add(type, 1);

        if (type == typeof(Rabbit))
        {
            Vector2 grid_position = spawning_tile.GetComponent<TileManager>().position;
            
            GameObject rabbit_go = GameObject.Instantiate(rabbitPrefab);

            rabbit_go.GetComponent<AnimalBehaviour>().SetSimulation(gameObject.GetComponent<SimulationManager>());
            rabbit_go.GetComponent<AnimalBehaviour>().position = grid_position;
            rabbit_go.GetComponent<AnimalBehaviour>().Initialize(definition_quality);

            rabbit_go.transform.parent = gameObject.transform;
            rabbit_go.transform.localScale = new Vector3(2f, 2f, 2f);

            GameObject center = spawning_tile.GetComponent<TileManager>().centerPlacement;

            Vector3 position = new Vector3(
                center.transform.position.x,
                center.transform.position.y + rabbit_go.GetComponent<MeshRenderer>().bounds.size.y / 2,
                center.transform.position.z
            );

            rabbit_go.transform.position = position;

            living_things_list.Add(rabbit_go);
        }
    }

    void AddPlant(GameObject spawning_tile, GameObject spawning_tile_empty, System.Type type)
    {
        // If the type is a key of populations, add 1 to it, else add it to the dictionary and set it to 1
        if (populations.ContainsKey(type))
            populations[type] += 1;
        else
            populations.Add(type, 1);

        if (type == typeof(Herb))
        {  
            GameObject herb_go = GameObject.Instantiate(herbPrefab);
            //rabbit_go.GetComponent<AnimalBehaviour>().SetSimulation(gameObject.GetComponent<SimulationManager>());
            //rabbit_go.GetComponent<AnimalBehaviour>().position = grid_position;
            //rabbit_go.GetComponent<AnimalBehaviour>().Initialize(definition_quality);
            herb_go.transform.parent = gameObject.transform;
            herb_go.transform.localScale = new Vector3(2f, 2f, 2f);
            
            Vector3 position = new Vector3(
                spawning_tile_empty.transform.position.x,
                spawning_tile_empty.transform.position.y + herb_go.GetComponent<MeshRenderer>().bounds.size.y / 2,
                spawning_tile_empty.transform.position.z
            );

            herb_go.transform.position = position;

            herb_go.GetComponent<MeshRenderer>().sharedMaterials = new Material[] {herbMaterial};

            // spawning_tile.GetComponent<TileManager>().Populate(herb_go); faire ça

            living_things_list.Add(herb_go);
        }
    }

    public TileType get_type(Vector2 position)  // Add height as an argument
    {
        float dist = distance_from_side(position);

        return dist < 0.3f? TileType.Grass : TileType.Rock;
    }

    public void PopulateTerrain()
    {
        var populations_random = new System.Random(seed);
        
        for (int x = 0; x < Mathf.Round(tiles.Count * tiles[0].Count / 25); x++)
        {
            int random_x = populations_random.Next(3, tiles.Count - 4);
            int random_y = populations_random.Next(3, tiles[random_x].Count - 4);

            GameObject tile = tiles[random_x][random_y];
            TileType type = tile.GetComponent<TileManager>().type;

            if (type == TileType.Grass)
            {
                AddAnimal(tile, typeof(Rabbit));
            }
        }
        
        for (int x = 2; x < tiles.Count - 3; x++)
        {
            for (int y = 2; y < tiles[x].Count - 3; y++)
            {
                float noise_value_1 = Mathf.PerlinNoise(x / 20 + seed * 7, y / 20 + seed * 7);
                float noise_value_2 = Mathf.PerlinNoise(x / 5 + seed * 14, y / 5 + seed * 14);

                float noise_value = 0.5f + (noise_value_1 * 0.75f + noise_value_2 * 0.25f) / 2f / 2f;
                int probability = (int)Mathf.Max(Mathf.Round(noise_value * 25f), 1);
                
                foreach (GameObject tile_empty_placement in tiles[x][y].GetComponent<TileManager>().placementPositions)
                {
                    if (tiles[x][y].GetComponent<TileManager>().type == TileType.Grass && populations_random.Next(0, probability + 1) <= 1)
                    {
                        AddPlant(tiles[x][y], tile_empty_placement, typeof(Herb));
                    }
                }

            }
        }
    }

    public void GenerateTerrain()
    {
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

        for (int i = 0; i < living_things_list.Count; i++)
        {
            DestroyImmediate(living_things_list[i]);
        }
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
        
        // two random noises combined together
        float first_noise_value = Mathf.PerlinNoise(position.x / (15f * smoothness) + diff_seed, position.y / (15f * smoothness) + diff_seed);
        float second_noise_value = Mathf.PerlinNoise(position.x / (8f * smoothness) + diff_seed, position.y / (8f * smoothness) + diff_seed);

        return (first_noise_value + second_noise_value) / 2f * 12f;
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
        // update(Time.deltaTime);
    }
}
