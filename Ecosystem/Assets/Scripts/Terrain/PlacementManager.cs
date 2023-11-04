using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public bool waterAccess = false;
    public float radius;

    public void CheckNearbyWater()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Tile")
            {
                if (collider.gameObject.GetComponent<TileManager>().under_water)
                {
                    waterAccess = true;
                }
            }
        }
    }

    public void Start()
    {
        CheckNearbyWater();
    }
}
