using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gender 
{
    Male,

    Female
}


public class Animal : LivingEntity
{
    public AnimalData data;
    public float size;
    public float speed;
    public float sensory_distance;
    public float gestation_duration;
    public float hunger;
    public float thirst;
    public int number_of_children;
    public float reproductive_urge;
    public Gender gender;
    public float desirability;
    public Dictionary<System.Enum, float> urge_to_run;
    public Vector2 objective;
    public List<Vector2> path;

    // public Animal reproduce(Animal partner) // pas encore testé
    // {
    //     // create an instance of the same animal as the parents
    //     System.Type parent_type = this.GetType();
    //     System.Reflection.ConstructorInfo constructor = parent_type.GetConstructor(new System.Type[] { typeof(SimulationManager) });

    //     Animal child = (Animal)constructor.Invoke(new object[] { this.simulation });
    //     child.position = this.position;

    //     // inherit the characteristics of the parents
    //     BellCurve speed_curve = new BellCurve((this.speed + partner.speed) / 2, 0.25f);
    //     child.speed = speed_curve.get_random_value();

    //     foreach (KeyValuePair<System.Type, float> entry in this.urge_to_run)
    //     {
    //         BellCurve urge_curve = new BellCurve((this.urge_to_run[entry.Key] + partner.urge_to_run[entry.Key]) / 2, 0.25f);
    //         child.urge_to_run[entry.Key] = urge_curve.get_random_value();
    //     }

    //     return child;
    // }

}

[System.Serializable]
public class Food
{
    public List<PlantType> plants = new List<PlantType>();
    public List<AnimalType> animals = new List<AnimalType>();
}