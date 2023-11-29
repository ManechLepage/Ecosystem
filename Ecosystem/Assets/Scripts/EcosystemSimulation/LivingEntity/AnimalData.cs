using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Entities/AnimalData")]
public class AnimalData : LivingEntityData
{
    public BellCurve number_of_children;
    public BellCurve gestation_duration;
    public BellCurve sensory_distance;
    [Header("The speed is 1/5 of speed in km/h")]
    public BellCurve speed;
    public Food can_eat;
    public Vector2 minMaxSize;
    public float maxHunger;
    public float maxThirst;
    public int reproductiveCoolDown;
    public float reproductiveMaturity;
    public float nutrition;
    [Space]
    [Header("Urges are numbers between 0 and 1 - auto : 1")]
    public Urge urge_to_run;
    public Urge urge_to_eat;
}

[System.Serializable]
public class Urge
{
    public List<PlantUrgeType> plants = new List<PlantUrgeType>();
    public List<AnimalUrgeType> animals = new List<AnimalUrgeType>();

    public Urge Clone()
    {
        Urge clone = new Urge();
        clone.plants = new List<PlantUrgeType>(plants);
        clone.animals = new List<AnimalUrgeType>(animals);
        return clone;
    }

    public bool Contains(PlantType type)
    {
        foreach (PlantUrgeType plant in plants)
        {
            if (plant.type == type)
                return true;
        }
        return false;
    }

    public bool Contains(AnimalType type)
    {
        foreach (AnimalUrgeType animal in animals)
        {
            if (animal.type == type)
                return true;
        }
        return false;
    }

    public float GetValue(PlantType type)
    {
        foreach (PlantUrgeType plant in plants)
        {
            if (plant.type == type)
                return plant.value;
        }
        return 0f;
    }

    public float GetValue(AnimalType type)
    {
        foreach (AnimalUrgeType animal in animals)
        {
            if (animal.type == type)
                return animal.value;
        }
        return 0f;
    }
}

[System.Serializable]
public class AnimalUrgeType
{
    public AnimalType type;
    public float value;
}

[System.Serializable]
public class PlantUrgeType
{
    public PlantType type;
    public float value;
}
