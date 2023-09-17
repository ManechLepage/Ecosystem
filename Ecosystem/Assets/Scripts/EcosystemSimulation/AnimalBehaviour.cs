using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimalBehaviour : MonoBehaviour
{
    public Vector2 position = new Vector2(0, 0);
    public float definition_quality = 1f;

    private SimulationManager simulation;
    public Animal simulated_animal;
    private UnityEngine.AI.NavMeshAgent agent;
    private Vector3 goal;

    void Start()
    {
        transform.localScale *= definition_quality;
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void Initialize(Animal simulated_animal, float definition_quality = 1f)
    {  
        this.simulated_animal = simulated_animal;
        // Debug.Log("Lifespan: " + simulated_animal.base_lifespan.mean + " " + simulated_animal.base_lifespan.standard_deviation + " " + simulated_animal.lifespan);
        Debug.Log("Number of childs: " + simulated_animal.number_of_childs);

        this.definition_quality = definition_quality;
        gameObject.transform.localScale *= this.definition_quality;

        simulation.living_things_list.Add(gameObject);
        GenerateRandomPosition();
    }

    public void SetSimulation(SimulationManager simulation)
    {
        this.simulation = simulation;
    }

    public void GenerateRandomPosition ()
    {
        int x_pos = Random.Range(0, (int)simulation.size.x);
        int y_pos = Random.Range(0, (int)simulation.size.y);
        goal = simulation.tiles[x_pos][y_pos].transform.position;

        // agent.destination = goal;  Bug for now
    }

    void Update()
    {
        //agent.destination = goal;  Bug for now
    }
}
