using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gender
{
    Male,
    Female
}

public class BellCurve
{
    public float mean;
    public float standard_deviation; // in percent

    public BellCurve(float mean, float standard_deviation)
    {
        this.mean = mean;
        this.standard_deviation = standard_deviation;
    }

    public float get_random_value()
    {
        float u1 = Random.Range(0f, 1f);
        float u2 = Random.Range(0f, 1f);
        float rand_std_normal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        float rand_normal = this.mean + this.standard_deviation * rand_std_normal;
        return rand_normal;
    }
}

public class LivingEntity
{
    public SimulationManager simulation;
    public Vector2 position;
    public BellCurve base_lifespan;
    public float lifespan;
    public float age;
    public List<float> growth_sizes;
    public float size;
    public float thirst;
    public string name;

    public LivingEntity(SimulationManager simulation, string name="être vivant")
    {
        this.simulation = simulation;
        this.position = new Vector2(0, 0);
        this.base_lifespan = new BellCurve(5, 0.25f); // Mean: 5, Standard deviation: 25%
        this.lifespan = this.base_lifespan.get_random_value();
        this.growth_sizes = new List<float> { 0.2f, 0.5f, 1f };
        this.size = 1f; //this.growth_sizes[0];
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
    public BellCurve base_number_of_childs;
    public int number_of_childs;
    public float gestation_duration;
    public float reproductive_urge;
    public Gender gender;
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
        this.base_number_of_childs = new BellCurve(3, 0.5f);
        this.number_of_childs = (int)Mathf.Round(this.base_number_of_childs.get_random_value());
        this.gestation_duration = 0f;
        this.reproductive_urge = 0f;
        this.gender = Random.Range(0, 1) == 1? Gender.Male: Gender.Female;
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

    public Animal reproduce(Animal partner) // pas encore testé
    {
        // create an instance of the same animal as the parents
        System.Type parent_type = this.GetType();
        System.Reflection.ConstructorInfo constructor = parent_type.GetConstructor(new System.Type[] { typeof(SimulationManager) });

        Animal child = (Animal)constructor.Invoke(new object[] { this.simulation });
        child.position = this.position;

        // inherit the characteristics of the parents
        BellCurve speed_curve = new BellCurve((this.speed + partner.speed) / 2, 0.25f);
        child.speed = speed_curve.get_random_value();

        foreach (KeyValuePair<System.Type, float> entry in this.urge_to_run)
        {
            BellCurve urge_curve = new BellCurve((this.urge_to_run[entry.Key] + partner.urge_to_run[entry.Key]) / 2, 0.25f);
            child.urge_to_run[entry.Key] = urge_curve.get_random_value();
        }

        return child;
    }
}

public class Rabbit : Animal
{
    public Rabbit(SimulationManager simulation, string name="lapin")
        : base(simulation, name)
    {
        // Définir les variables pour le lapin

        LivingType living_type = new LivingType(); // enlever ça quand on aura un enum

        this.base_lifespan = new BellCurve(5, 0.25f);
        this.lifespan = this.base_lifespan.get_random_value();
        this.growth_sizes = new List<float> { 0.2f, 0.5f, 0.8f, 0.9f, 1f };
        this.size = 1f; //this.growth_sizes[0];
        this.sensory_distance = 5f;
        this.speed = 1f;
        this.gestation_duration = 50f;
        this.base_number_of_childs = new BellCurve(3, 0.5f);
        this.number_of_childs = (int)Mathf.Round(this.base_number_of_childs.get_random_value());
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
    {
        this.growth_sizes = new List<float> { 0.6f, 1f, 1.5f };
    }
}

public class OakTree : Plant
{
    public OakTree(SimulationManager simulation, string name="chêne")
        : base(simulation, name)
    {
        this.growth_sizes = new List<float> { 0.7f, 1.1f, 1.6f };
    }
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