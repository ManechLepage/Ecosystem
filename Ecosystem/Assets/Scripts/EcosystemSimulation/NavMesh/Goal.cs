using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter3D(Collider other)
    {
        other.GetComponent<NavigationManager>().SetGoal();
        Debug.Log("Goal Reached!");
    }
}
