using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public int seed = -1; // If -1, the seed is random
    public Vector2 size = new Vector2(64, 64);
    
    public Simulation simulation;

    public BiomeGenerator biome;
    
    // Start is called before the first frame update
    void Start()
    {
        if (seed == -1)
        {
            seed = Random.Range(1, 1_000_000_000);
        }
        
        this.simulation = new Simulation(size, seed);
        this.biome = new BiomeGenerator("test", seed, new Dictionary<System.Type, Dictionary<System.Type, int>>());
        this.simulation.biome = this.biome;
        this.simulation.generate();

        // TODO: Add map tiles generation
    }

    void AddLivingThing(Vector2 position, System.Type type)
    {
        // TODO
    }

    // Update is called once per frame
    void Update()
    {
        this.simulation.update(Time.deltaTime);
    }
}
