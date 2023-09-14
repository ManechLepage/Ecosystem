using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Tile Prefabs")]
    [SerializeField] public Material grassTile;
    [SerializeField] public Material sandTile;
    [SerializeField] public Material rockTile;
    [Space]
    public GameObject tilePrefab;

    private Dictionary<TileType, Material> tileMaterials;
    
    public List<GameObject> living_things_list;
    public float time;
    public List<List<GameObject>> tiles;

    public string biomeName;
    public Dictionary<System.Type, Dictionary<System.Type, int>> populations;
    public bool add_walls = true;
    
    public void Start()
    {

        tiles = new List<List<GameObject>>();

        tileMaterials = new Dictionary<TileType, Material>() {
            {TileType.Grass, grassTile},
            {TileType.Sand, sandTile},
            {TileType.Rock, rockTile}
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

    void AddLivingThing(Vector2 position, System.Type type)
    {
        // TODO
    }

    public virtual TileType get_type(Vector2 position)  // Add height as an argument
    {
        return Random.Range(0, 2) == 1? TileType.Grass : Random.Range(0, 2) == 1? TileType.Sand : TileType.Rock;
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
                // Material material = grassTile;

                GameObject tile = GameObject.Instantiate(tilePrefab);
                tile.transform.parent = gameObject.transform;

                // tile.GetComponent<Renderer>().material = material;
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
                    tile.transform.localScale.z * tile_size * definition_quality);  

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
    }

    // The two next functions are used to create walls around the map
    
    private float distance_from_side(Vector2 position)
    {
        float diff_seed = seed * 5f;
        
        float distance_from_side_x = Mathf.Min(position.x, size.x - position.x - 1);
        float distance_from_side_y = Mathf.Min(position.y, size.y - position.y - 1);

        float distance_from_side = Mathf.Min(distance_from_side_x, distance_from_side_y);

        float final_value = Mathf.Pow(Mathf.Max(1f - distance_from_side / 6, 0f), 3.5f);
        final_value *=  1 + (Mathf.PerlinNoise(position.x / 10f + position.y / 10f + diff_seed, final_value + diff_seed) / 2f - 0.25f);

        return final_value;
    }

    public float get_height(Vector2 position)
    {
        float diff_seed = seed * 10f;
        
        // two random noises combined together
        float first_noise_value = Mathf.PerlinNoise(position.x / 15f + diff_seed, position.y / 15f + diff_seed);
        float second_noise_value = Mathf.PerlinNoise(position.x / 8f + diff_seed, position.y / 8f + diff_seed);

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

        return get_height(position) + distance_from_side_value * 17f;
    } 


    void AddTile(Vector2 position, float height)
    {
        // TODO
    }

    // Update is called once per frame
    void Update()
    {
        // update(Time.deltaTime);
    }
}
