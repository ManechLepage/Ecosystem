using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBehaviour : MonoBehaviour
{
    public Vector2 position = new Vector2(0, 0);
    public float definition_quality = 1f;
    public List<GameObject> growth_stages = new List<GameObject>();

    private SimulationManager simulation;
    public Plant simulated_living;
    
    public void Initialize(Plant simulated_living, float definition_quality = 1f)
    {
        this.simulated_living = simulated_living;

        this.definition_quality = definition_quality;
        gameObject.transform.localScale *= this.definition_quality;

        simulation.living_things_list.Add(gameObject);
    }

    public void SetSimulation(SimulationManager simulation)
    {
        this.simulation = simulation;
    }

    void Update()
    {
        
    }
}
