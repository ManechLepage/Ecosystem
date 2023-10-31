using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] placementPositions = new GameObject[6];
    public GameObject centerPlacement;
    public GameObject centerPopulation;
    public GameObject[] tilePopulation = new GameObject[6];

    public Vector2 position;
    public float height;
    public TileType type = TileType.Grass;
    public bool under_water = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Populate(int placement, GameObject obj)
    {
        tilePopulation[placement] = obj;
    }
}