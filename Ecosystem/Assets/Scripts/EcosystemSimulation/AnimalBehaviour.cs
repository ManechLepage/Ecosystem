using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimalBehaviour : MonoBehaviour
{
    public Simulation simulation;
    public Vector2 position = new Vector2(0, 0);

    private Animal simulated_animal;

    // Start is called before the first frame update
    void Start()
    {   
        LivingType living_type = new LivingType();
        // Generate the animal's settings
        Vector2 lifespan = new Vector2(5, 2);
        float sensory_distance = 5f;
        float speed = 1f;
        float reproductive_urge = 0f;
        float gestation_duration = 50f;
        Vector2 number_of_childs = new Vector2(3, 2);
        float desirability = 0.5f;

        Dictionary<System.Type, float> urge_to_run = new Dictionary<System.Type, float> {
            { living_type.rabbit, 0f },
            { living_type.fox, 0.75f },
            { living_type.herb, 0f }
        };

        Dictionary<System.Type, List<System.Type>> can_eat = new Dictionary<System.Type, List<System.Type>> {
            { living_type.animal, new List<System.Type>
                { } },
            { living_type.plant, new List<System.Type>
                { living_type.herb } }
        };

        simulated_animal = new Rabbit(
            simulation,
            position,
            lifespan,
            new List<float> { 0.2f, 0.5f, 0.8f, 0.9f, 1f },
            sensory_distance,
            speed,
            reproductive_urge,
            gestation_duration,
            number_of_childs,
            desirability,
            urge_to_run,
            can_eat);
        
        // Error because the simulation should be defined in the SimulationManager before the animal is created
        simulation.living_things_list.Add(simulated_animal);
        
        // Log all the animals of the simulation with Debug
        foreach (Animal animal in simulation.living_things_list)
        {
            Debug.Log(animal);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
