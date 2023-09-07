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

    public void generate(Simulation simulation)
    {
        float x_pos = 0;
        float y_pos = 0;

        for (int x = 0; x < simulation.size.x; x++)
        {
            List<Tile> column = new List<Tile>();
            for (int y = 0; y < simulation.size.y; y++)
            {
                float offset = (x % 2 == 0 ? Mathf.Sqrt(0.75f) : 0f);
                
                column.Add(new Tile(
                    simulation,
                    new Vector2(x_pos + offset, y_pos),
                    Mathf.Round(get_real_height(new Vector2(x, y), simulation))
                ));

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


public class Tile
{
    public Simulation simulation;
    public Vector2 position;
    public float height;

    public Tile(Simulation simulation, Vector2 position, float height)
    {
        this.simulation = simulation;
        this.position = position;
        this.height = height;
    }
}


public class Simulation
{
    public List<LivingEntity> living_things_list;
    public float time;
    public List<List<Tile>> tiles;
    public float tile_size;
    public Vector2 size;
    public Dictionary<System.Type, int> populations;
    public int seed;
    public BiomeGenerator biome;

    public Simulation(Vector2 size, int seed=0, float tile_size=1f)
    {
        this.seed = seed;
        this.time = 0;
        this.size = size;
        this.tiles = new List<List<Tile>>();
        this.tile_size = tile_size;
        this.living_things_list = new List<LivingEntity>();
        this.populations = new Dictionary<System.Type, int>();
    }

    public void generate()
    {
        this.biome.generate(this);
    }

    public void update(float delta_time)
    {
        this.time += delta_time;
        foreach (LivingEntity living_thing in this.living_things_list)
        {
            living_thing.update(delta_time);
        }
    }
}


public class LivingEntity
{
    public Simulation simulation;
    public Vector2 position;
    public Vector2 lifespan;
    public float age;
    public List<float> growth_sizes;
    public float thirst;
    public string name;

    public LivingEntity(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes, string name="être vivant")
    {
        this.simulation = simulation;
        this.position = position;
        this.lifespan = lifespan;
        this.growth_sizes = growth_sizes;
        this.age = 0;
        this.thirst = 0;
        this.name = name;
    }

    public void update(float delta_time)
    {
        this.age += delta_time;
    }
}


public class Plant : LivingEntity
{
    public Plant(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes, string name="plante")
        : base(simulation, position, lifespan, growth_sizes, name)
    { }
}


public class Animal : LivingEntity
{
    public float sensory_distance;
    public float speed;
    public float hunger;
    public Vector2 number_of_childs;
    public float desirability;
    public Dictionary<System.Type, float> urge_to_run;
    public Dictionary<System.Type, List<System.Type>> can_eat;
    public Vector2 objective;
    public List<Vector2> path;

    public Animal(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes, float sensory_distance, float speed,
        float reproductive_urge, float gestation_duration, Vector2 number_of_childs,
        float desirability, Dictionary<System.Type, float> urge_to_run,
        Dictionary<System.Type, List<System.Type>> can_eat, string name="animal")
        : base(simulation, position, lifespan, growth_sizes, name)
    {
        this.sensory_distance = sensory_distance;
        this.speed = speed;
        this.hunger = 0;
        this.number_of_childs = number_of_childs;
        this.desirability = desirability;
        this.urge_to_run = urge_to_run;
        this.can_eat = can_eat;
        this.objective = position;
        this.path = new List<Vector2>();
    }

    public void find_path(Vector2 objective)
    {
        this.objective = objective;
        this.path = new List<Vector2>();
        // TODO
    }

    public void update_movement(float delta_time)
    {
        // TODO
    }

    public new void update(float delta_time)
    {
        base.update(delta_time);
        // TODO
    }
}

public class Rabbit : Animal
{
    public Rabbit(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes, float sensory_distance, float speed,
        float reproductive_urge, float gestation_duration, Vector2 number_of_childs,
        float desirability, Dictionary<System.Type, float> urge_to_run,
        Dictionary<System.Type, List<System.Type>> can_eat, string name="lapin")
        : base(simulation, position, lifespan, growth_sizes, sensory_distance, speed,
            reproductive_urge, gestation_duration, number_of_childs, desirability, urge_to_run,
            can_eat, name)
    {

    }
}

public class Fox : Animal
{
    public Fox(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes, float sensory_distance, float speed,
        float reproductive_urge, float gestation_duration, Vector2 number_of_childs,
        float desirability, Dictionary<System.Type, float> urge_to_run,
        Dictionary<System.Type, List<System.Type>> can_eat, string name="renard")
        : base(simulation, position, lifespan, growth_sizes, sensory_distance, speed,
            reproductive_urge, gestation_duration, number_of_childs, desirability, urge_to_run,
            can_eat)
    {

    }
}

public class Herb : Plant
{
    public Herb(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes, string name="herbe")
        : base(simulation, position, lifespan, growth_sizes, name)
    { }
}


public class LivingType
{
    public System.Type animal = typeof(Animal);
    public System.Type plant = typeof(Plant);
    
    public System.Type rabbit = typeof(Rabbit);
    public System.Type fox = typeof(Fox);
    public System.Type herb = typeof(Herb);
}

public class Mammal
{
    public float reproductive_urge;
    public float gestation_duration;
    public Dictionary<string, int> number_of_childs;
}


// Voir si on fais ça
public class Vegetarian
{

}

public class Carnivore
{

}

public class Ominvore
{

}