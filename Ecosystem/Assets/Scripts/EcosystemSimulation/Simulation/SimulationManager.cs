using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public int seed = -1; // If -1, the seed is random
    public Vector2 size = new Vector2(64, 64);
    [SerializeField] private GameObject tile;
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
        simulation.generate();

        // Add the tiles to the scene using the prefab in : Assets/Prefabs/EcosystemSimulation/Tiles/HexagonalTile.prefab
        for (int x = 0; x < simulation.size.x; x++)
        {
            for (int y = 0; y < simulation.size.y; y++)
            {
                Tile tile_info = simulation.tiles[x][y];
                GameObject tile_go = Instantiate(
                    tile,
                    new Vector3(
                        tile_info.position.x * definition_quality * simulation.tile_size,
                        tile_info.height * definition_quality,
                        tile_info.position.y * definition_quality * simulation.tile_size),
                    Quaternion.identity);
                
                tile_go.transform.Rotate(new Vector3(1, 0, 0), -90);
                tile_go.transform.parent = this.transform;

                tile_go.transform.localScale = new Vector3(
                    tile_go.transform.localScale.x * simulation.tile_size * definition_quality,
                    tile_go.transform.localScale.y * simulation.tile_size * definition_quality,
                    tile_go.transform.localScale.z * simulation.tile_size * definition_quality);                
            }
        }
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
