using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntityData : ScriptableObject
{
    public BellCurve lifespan;
    public string objectName;
    public List<float> growth_sizes;
    public List<Mesh> growth_stages;
    public System.Enum type;
}

[CreateAssetMenu(fileName = "AnimalData", menuName = "Entities/AnimalData")]
public class AnimalData : LivingEntityData
{
    public BellCurve number_of_children;
    public BellCurve gestation_duration;
    public BellCurve sensory_distance;
    public BellCurve desirability;
    public BellCurve speed;
    public Food can_eat;
}

[CreateAssetMenu(fileName = "PlantData", menuName = "Entities/PlantData")]
public class PlantData : LivingEntityData
{

}
