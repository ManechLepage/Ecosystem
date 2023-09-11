using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] placementPositions = new GameObject[5];
    public GameObject[] tilePopulation = new GameObject[5];

    public Vector2 position;
    public float height;
    public TileType type = TileType.Grass;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void Populate(int placement, GameObject obj)
    {
        tilePopulation[placement] = obj;
    }
}