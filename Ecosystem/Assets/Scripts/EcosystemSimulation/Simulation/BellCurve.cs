using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BellCurve
{
    public float mean;
    public float standard_deviation; // in percent

    public BellCurve(float mean, float standard_deviation)
    {
        this.mean = mean;
        this.standard_deviation = standard_deviation;
    }

    public float get_random_value()
    {
        float u1 = Random.Range(0f, 1f);
        float u2 = Random.Range(0f, 1f);
        float rand_std_normal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        float rand_normal = this.mean + this.standard_deviation * rand_std_normal;
        return rand_normal;
    }
}
