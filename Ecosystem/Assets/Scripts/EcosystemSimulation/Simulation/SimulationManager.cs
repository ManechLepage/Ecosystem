using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public int seed = -1; // If -1, the seed is random
    public Vector2 size = new Vector2(64, 64);
    [SerializeField] public GameObject tile;
    public float tile_size = 1f;
    public float definition_quality = 5f;
    public bool add_walls = true;
    
    public Simulation simulation;

    public BiomeGenerator biome;
    
    // Start is called before the first frame update
    void Start()
    {
        if (seed == -1)
        {
            seed = Random.Range(1, 100_000); // DO NOT make this number bigger, it will cause terrain generation bugs
        }
        
        simulation = new Simulation(size, seed, tile_size);
        biome = new PlanesBiomeGenerator(new Dictionary<System.Type, Dictionary<System.Type, int>>(), add_walls);
        simulation.biome = biome;
        GenerateTerrain();

        // Call TempStart from the file of the rabbit gameobject
        GameObject rabbit_go = GameObject.Find("Rabbit");
        rabbit_go.GetComponent<AnimalBehaviour>().SetSimulation(this.simulation);
        rabbit_go.GetComponent<AnimalBehaviour>().Initialize();

        // TODO: Add map tiles generation
    }

    void AddLivingThing(Vector2 position, System.Type type)
    {
        // TODO
    }

    void GenerateTerrain()
    {
        simulation.generate(tile, definition_quality);
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
