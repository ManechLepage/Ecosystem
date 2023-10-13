using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    public List<Transform> goals;
    private Vector3 goal;
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetGoal();
    }

    void Update()
    {
        agent.destination = goal;
    }

    public void SetGoal()
    {
        goal = goals[Random.Range(0, goals.Count)].position;
    }
    
    private void OnGUI()
    {
        if (GUILayout.Button("Generate New Goal"))
        {
            SetGoal();
        }
    }
}
