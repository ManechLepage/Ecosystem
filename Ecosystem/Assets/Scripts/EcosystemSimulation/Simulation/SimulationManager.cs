using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("General Settings")]
    public int seed = -1; // If -1, the seed is random
    public Vector2 size = new Vector2(64, 64);

    [Header("Terrain Settings")]
    public float tile_size = 1f;
    public float definition_quality = 5f;
    public bool add_walls = true;

    [Header("Tile Prefabs")]
    [SerializeField] public GameObject grassTile;
    [SerializeField] public GameObject sandTile;
    [SerializeField] public GameObject rockTile;

    private Dictionary<TileType, GameObject> tilePrefabs;
    
    public Simulation simulation;

    public BiomeGenerator biome;
    
    // Start is called before the first frame update
    void Start()
    {
        tilePrefabs = new Dictionary<TileType, GameObject>() {
            {TileType.Grass, grassTile},
            {TileType.Sand, sandTile},
            {TileType.Rock, rockTile}
        }; 
        
        if (seed == -1)
        {
            seed = Random.Range(1, 100_000); // DO NOT make this number bigger, it will cause terrain generation bugs
        }
        
        simulation = new Simulation(size, seed, tile_size);
        biome = new PlanesBiomeGenerator(new Dictionary<System.Type, Dictionary<System.Type, int>>(), add_walls);
        simulation.biome = biome;
        GenerateTerrain();

        // Temporary : add a rabbit
        GameObject rabbit_go = GameObject.Find("Rabbit");
        rabbit_go.GetComponent<AnimalBehaviour>().SetSimulation(this.simulation);
        rabbit_go.GetComponent<AnimalBehaviour>().Initialize();

    }

    void AddLivingThing(Vector2 position, System.Type type)
    {
        // TODO
    }

    void GenerateTerrain()
    {
        simulation.generate(tilePrefabs, definition_quality);
    }

    void AddTile(Vector2 position, float height)
    {
        // TODO
    }

    // Update is called once per frame
    void Update()
    {
        simulation.update(Time.deltaTime);
    }
}
