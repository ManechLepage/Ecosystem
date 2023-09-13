using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator
{
    public string name;
    public Dictionary<System.Type, Dictionary<System.Type, int>> populations;
    public bool add_walls = true;

    public BiomeGenerator(string name, Dictionary<System.Type, Dictionary<System.Type, int>> populations, bool add_walls = true)
    {
        this.name = name;
        this.populations = populations;
        this.add_walls = add_walls;
    }

    public void add_population(Simulation simulation)
    {
        // TODO
    }

    public virtual float get_height(Vector2 position, Simulation simulation)
    {
        return 0f;
    }

    public virtual TileType get_type(Vector2 position, Simulation simulation)  // Add height as an argument
    {
        return Random.Range(0, 2) == 1? TileType.Grass : Random.Range(0, 2) == 1? TileType.Sand : TileType.Rock;
    }

    public void generate(Simulation simulation, Dictionary<TileType, Material> tileMaterials, float definition_quality, GameObject tilePrefab)
    {
        float x_pos = 0;
        float y_pos = 0;

        for (int x = 0; x < simulation.size.x; x++)
        {
            List<GameObject> column = new List<GameObject>();
            for (int y = 0; y < simulation.size.y; y++)
            {
                float offset = (x % 2 == 0 ? Mathf.Sqrt(0.75f) : 0f);
                TileType type = get_type(new Vector2(x, y), simulation);
                Material material = tileMaterials[type];
                GameObject tile = GameObject.Instantiate(tilePrefab);
                tile.GetComponent<Renderer>().material = material;
                
                TileManager tileInfo = tile.GetComponent<TileManager>();
                tileInfo.position = new Vector2(x_pos + offset, y_pos);
                tileInfo.height = Mathf.Round(get_real_height(new Vector2(x, y), simulation));

                tile.transform.position = new Vector3(
                        tileInfo.position.x * definition_quality * simulation.tile_size,
                        tileInfo.height * definition_quality,
                        tileInfo.position.y * definition_quality * simulation.tile_size);

                tile.transform.localScale = new Vector3(
                    tile.transform.localScale.x * simulation.tile_size * definition_quality,
                    tile.transform.localScale.y * simulation.tile_size * definition_quality,
                    tile.transform.localScale.z * simulation.tile_size * definition_quality);  

                column.Add(tile);
                x_pos += Mathf.Sqrt(3f);
            }
            simulation.tiles.Add(column);
            y_pos += 0.75f * 2f;
            x_pos = 0;
        }
    }

    // The two next functions are used to create walls around the map
    
    private float distance_from_side(Vector2 position, Simulation simulation)
    {
        float diff_seed = simulation.seed * 5f;
        
        float distance_from_side_x = Mathf.Min(position.x, simulation.size.x - position.x - 1);
        float distance_from_side_y = Mathf.Min(position.y, simulation.size.y - position.y - 1);

        float distance_from_side = Mathf.Min(distance_from_side_x, distance_from_side_y);

        float final_value = Mathf.Pow(Mathf.Max(1f - distance_from_side / 6, 0f), 3.5f);
        final_value *=  1 + (Mathf.PerlinNoise(position.x / 10f + position.y / 10f + diff_seed, final_value + diff_seed) / 2f - 0.25f);

        return final_value;
    }

    private float get_real_height(Vector2 position, Simulation simulation)
    {
        // Here we want to use distance_from_side to modify the height of the tile, to create walls around the map
        // To do this, the distance from side value will be modified so the walls get a high angle (using a power function)

        float distance_from_side_value = 0f;
        if (add_walls)
        {
            distance_from_side_value = distance_from_side(position, simulation);
        } 

        return get_height(position, simulation) + distance_from_side_value * 17f;
    } 
}

public class PlanesBiomeGenerator : BiomeGenerator
{   
    public PlanesBiomeGenerator(
        Dictionary<System.Type, Dictionary<System.Type, int>> populations,
        bool add_walls=true
    ) : base("plaine", populations, add_walls){}
    
    public override float get_height(Vector2 position, Simulation simulation)
    {
        float diff_seed = simulation.seed * 10f;
        
        // two random noises combined together
        float first_noise_value = Mathf.PerlinNoise(position.x / 15f + diff_seed, position.y / 15f + diff_seed);
        float second_noise_value = Mathf.PerlinNoise(position.x / 8f + diff_seed, position.y / 8f + diff_seed);

        return (first_noise_value + second_noise_value) / 2f * 12f;
    }
}