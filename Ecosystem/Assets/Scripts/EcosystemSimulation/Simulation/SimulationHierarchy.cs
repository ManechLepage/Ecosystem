using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Grass,
    Sand,
    Rock
}


public class Simulation
{
    public List<GameObject> living_things_list;
    public float time;
    public List<List<GameObject>> tiles;
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
        this.tiles = new List<List<GameObject>>();
        this.tile_size = tile_size;
        this.living_things_list = new List<GameObject>();
        this.populations = new Dictionary<System.Type, int>();
    }

    public void generate(Dictionary<TileType, GameObject> tilePrefabs, float definition_quality)
    {
        this.biome.generate(this, tilePrefabs, definition_quality);
    }

    public void update(float delta_time)
    {
        this.time += delta_time;
        foreach (GameObject living_thing in this.living_things_list)
        {
            living_thing.GetComponent<AnimalBehaviour>().simulated_animal.update(delta_time);
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