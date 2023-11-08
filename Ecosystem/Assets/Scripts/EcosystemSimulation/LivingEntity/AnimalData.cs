using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Entities/AnimalData")]
public class AnimalData : LivingEntityData
{
    public BellCurve number_of_children;
    public BellCurve gestation_duration;
    public BellCurve sensory_distance;
    public BellCurve speed;
    public Food can_eat;
    public Vector2 minMaxSize;
    public float maxHunger;
    public float maxThirst;
    public int reproductiveCoolDown;
    public float reproductiveMaturity;
}

