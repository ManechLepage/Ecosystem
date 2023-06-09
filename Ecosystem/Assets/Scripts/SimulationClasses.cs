using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class BiomeGenerator
{
    public string name;
    public int seed;
    public Dictionary<string, Dictionary<string, int>> populations;

    public BiomeGenerator(string name, int seed, Dictionary<string, Dictionary<string, int>> populations)
    {
        this.name = name;
        this.seed = seed;
        this.populations = populations;
    }

    public void add_population(Simulation simulation)
    {
        // TODO
    }

    public void generate(Simulation simulation)
    {
        // TODO
    }
}


class Simulation
{
    public List<LivingEntity> living_thing_list;
    public float time;
    public Grid grid;
    public Dictionary<LivingEntity, int> populations;
    public int seed;
    public BiomeGenerator biome;

    public Simulation(int seed = 0)
    {
        this.seed = seed;
        this.time = 0;
        this.living_thing_list = new List<LivingEntity>();
        this.populations = new Dictionary<LivingEntity, int>();
    }

    public void generate()
    {
        this.biome.generate(this);
    }

    public void update(float delta_time)
    {
        this.time += delta_time;
        foreach (LivingEntity living_thing in this.living_thing_list)
        {
            living_thing.update(delta_time);
        }
    }
}


class LivingEntity
{
    public Simulation simulation;
    public Vector2 position;
    public Vector2 lifespan;
    public float age;
    public List<float> growth_sizes;
    public float thirst;

    public LivingEntity(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes)
    {
        this.simulation = simulation;
        this.position = position;
        this.lifespan = lifespan;
        this.growth_sizes = growth_sizes;
        this.age = 0;
        this.thirst = 0;
    }

    public void update(float delta_time)
    {
        this.age += delta_time;
    }
}


class Plant : LivingEntity
{
    public Plant(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes)
        : base(simulation, position, lifespan, growth_sizes)
    { }
}


class Animal : LivingEntity
{
    public float sensory_distance;
    public float speed;
    public float hunger;
    public float desirability;
    public Dictionary<string, float> urge_to_run;
    public Dictionary<string, List<string>> can_eat;
    public Vector2 objective;
    public List<Vector2> path;

    public Animal(Simulation simulation, Vector2 position, Vector2 lifespan,
        List<float> growth_sizes, float sensory_distance, float speed,
        float reproductive_urge, float gestation_duration, Dictionary<string, int> number_of_childs,
        float desirability, Dictionary<string, float> urge_to_run,
        Dictionary<string, List<string>> can_eat)
        : base(simulation, position, lifespan, growth_sizes)
    {
        this.sensory_distance = sensory_distance;
        this.speed = speed;
        this.hunger = 0;
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

class Mammal
{
    public float reproductive_urge;
    public float gestation_duration;
    public Dictionary<string, int> number_of_childs;
}

class Vegetarian 
{

}

class Carnivore
{

}

class Ominvore
{
    
}