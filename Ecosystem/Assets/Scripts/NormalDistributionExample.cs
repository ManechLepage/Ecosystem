using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDistributionExample : MonoBehaviour
{

    public float min;
    public float max;

    public GameObject tree;

    float GenerateRandomNormal(float mean, float stdDev)
    {
        float u1 = 1f - Random.value; // Generate a random value between 0 and 1 (exclusive)
        float u2 = 1f - Random.value;

        // Box-Muller transform
        float z0 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);

        // Scale and shift to match the desired mean and standard deviation
        float randomNumber = mean + stdDev * z0;

        return randomNumber;
    }

    float GenerateDistribution(float minRange, float maxRange)
    {
        float rangeExpansionFactor = 0.2f;

        float randomValue = GenerateRandomNormal((minRange + maxRange)/2, (maxRange - minRange) * rangeExpansionFactor);

        return randomValue;
    }

    void Start()
    {  
        List<float> values = new List<float>();

        for (int i = 0; i < 1000; i++)
        {
            values.Add(Mathf.Round(GenerateDistribution(min, max) * 10));
        }

        values.Sort();

        for (int i = 0; i < 1000; i++)
        {
            Vector3 position = new Vector3(i/10, values[i], 0);
            GameObject trees = Instantiate(tree, position, Quaternion.identity);
        }
        
    }
    
}

