using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "Biomes/BiomeData")]
public class BiomeData : ScriptableObject
{
    public BiomeType type;
    [Header("Terrain")]
    public float smoothness = 1f;
    public float intensity = 1f;
    public float waterLevel = 1f;
    [Header("Populations Proportions")]
    public Populations populations = new Populations();
    public float animalDensity = 1f;
    public float plantDensity = 1f;
}

[System.Serializable]
public class Populations
{
    public List<AnimalPopulation> animalPopulations = new List<AnimalPopulation>();
    public List<PlantPopulation> plantPopulations = new List<PlantPopulation>();

    public Populations()
    {
        foreach (AnimalType animalType in System.Enum.GetValues(typeof(AnimalType)))
        {
            AnimalPopulation population = new AnimalPopulation();
            population.type = animalType;
            animalPopulations.Add(population);
        }

        foreach (PlantType plantType in System.Enum.GetValues(typeof(PlantType)))
        {
            PlantPopulation population = new PlantPopulation();
            population.type = plantType;
            plantPopulations.Add(population);
        }
    }

    public List<AnimalType> GetAnimals()
    {
        List<AnimalType> animals = new List<AnimalType>();

        foreach (AnimalPopulation population in animalPopulations)
        {
            animals.Add(population.type);
        }

        return animals;
    }

    public List<PlantType> GetPlants()
    {
        List<PlantType> plants = new List<PlantType>();

        foreach (PlantPopulation population in plantPopulations)
        {
            plants.Add(population.type);
        }

        return plants;
    }

    public List<int> GetAnimalsWeights()
    {
        List<int> weights = new List<int>();

        foreach (AnimalPopulation population in animalPopulations)
        {
            weights.Add(population.population);
        }

        return weights;
    }

    public List<int> GetPlantsWeights()
    {
        List<int> weights = new List<int>();

        foreach (PlantPopulation population in plantPopulations)
        {
            weights.Add(population.population);
        }

        return weights;
    }

    public AnimalType GetRandomAnimal(System.Random definedRandom=null)
    {
        if (definedRandom == null)
        {
            definedRandom = new System.Random();
        }

        int total = 0;
        List<AnimalType> animalTypes = GetAnimals();
        List<int> weights = GetAnimalsWeights();

        foreach (int weight in weights)
        {
            total += weight;
        }

        int random = definedRandom.Next(0, total);
        int current = 0;

        for (int i = 0; i < animalTypes.Count; i++)
        {
            current += weights[i];

            if (current > random)
            {
                return animalTypes[i];
            }
        }

        return animalTypes[animalTypes.Count - 1];
    }

    public PlantType GetRandomPlant(System.Random definedRandom=null)
    {
        if (definedRandom == null)
        {
            definedRandom = new System.Random();
        }

        int total = 0;
        List<PlantType> plantTypes = GetPlants();
        List<int> weights = GetPlantsWeights();

        foreach (int weight in weights)
        {
            total += weight;
        }

        int random = definedRandom.Next(0, total);
        int current = 0;

        for (int i = 0; i < plantTypes.Count; i++)
        {
            current += weights[i];

            if (current > random)
            {
                return plantTypes[i];
            }
        }

        return plantTypes[plantTypes.Count - 1];
    }
}

[System.Serializable]
public class AnimalPopulation
{
    public AnimalType type;
    public int population = 1;
}

[System.Serializable]
public class PlantPopulation
{
    public PlantType type;
    public int population = 1;
}
