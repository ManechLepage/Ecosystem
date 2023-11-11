using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPlacements : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Animal")
        {
            if (other.gameObject.GetComponent<Entity>().livingEntity is Animal animal)
                animal.FoundObjective();
        }
    }
}
