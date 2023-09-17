using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity
{
    public SimulationManager simulation;
    public Vector2 position;
    public Vector2 lifespan;
    public float age;
    public List<float> growth_sizes;
    public float thirst;
    public string name;

    public LivingEntity(SimulationManager simulation, string name="être vivant")
    {
        this.simulation = simulation;
        this.position = new Vector2(0, 0);
        this.lifespan = new Vector2(5, 2);
        this.growth_sizes = new List<float> { 0.2f, 0.5f, 0.8f, 0.9f, 1f };
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
    public Plant(SimulationManager simulation, string name="plante")
        : base(simulation, name)
    { }
}


public class Animal : LivingEntity
{
    public float sensory_distance;
    public float speed;
    public float hunger;
    public Vector2 number_of_childs;
    public float gestation_duration;
    public float reproductive_urge;
    public float desirability;
    public Dictionary<System.Type, float> urge_to_run;
    public Dictionary<System.Type, List<System.Type>> can_eat;
    public Vector2 objective;
    public List<Vector2> path;

    public Animal(SimulationManager simulation, string name="animal")
        : base(simulation, name)
    {
        this.sensory_distance = 5f;
        this.speed = 1f;
        this.hunger = 0;
        this.number_of_childs = new Vector2(3, 2);
        this.gestation_duration = 0f;
        this.reproductive_urge = 0f;
        this.desirability = 0.5f;
        this.urge_to_run = new Dictionary<System.Type, float>();
        this.can_eat = new Dictionary<System.Type, List<System.Type>>();
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
    public Rabbit(SimulationManager simulation, string name="lapin")
        : base(simulation, name)
    {
        // Définir les variables pour le lapin

        LivingType living_type = new LivingType(); // enlever ça quand on aura un enum

        this.lifespan = new Vector2(5, 2);
        this.sensory_distance = 5f;
        this.speed = 1f;
        this.gestation_duration = 50f;
        this.number_of_childs = new Vector2(3, 2);
        this.desirability = 0.5f;

        this.urge_to_run = new Dictionary<System.Type, float> {
            { living_type.rabbit, 0f },
            { living_type.fox, 0.75f },
            { living_type.herb, 0f }
        };

        this.can_eat = new Dictionary<System.Type, List<System.Type>> {
            { living_type.animal, new List<System.Type>
                { } },
            { living_type.plant, new List<System.Type>
                { living_type.herb } }
        };
    }
}

public class Fox : Animal
{
    public Fox(SimulationManager simulation, string name="renard")
        : base(simulation, name)
    {
        
    }
}

public class Herb : Plant
{
    public Herb(SimulationManager simulation, string name="herbe")
        : base(simulation, name)
    { }
}


public class LivingType // changer ça pour un enum
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