using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSelectionDetection : MonoBehaviour
{
    void Update()
    {
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null) {
            Debug.Log ("CLICKED " + hit.collider.name);
        }
    }
    }
}
