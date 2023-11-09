using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendInfoToSheets : MonoBehaviour
{
    [SerializeField]
    private string ANIMAL_BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLScQczyRS2A9Wl-xGJxcH2lL1fYEmrtj64ppIKL8xpXjBU_a8w/formResponse";   
    [SerializeField]
    private string SIMULATION_BASE_URL = "";
    IEnumerator PostAnimalData(int simulationAge, string animalType, float age, float speed, float thirst, float hunger, int population)
    {
        WWWForm form = new WWWForm();

        form.AddField("entry.1681844743", simulationAge.ToString());
        form.AddField("entry.704130423", animalType);
        form.AddField("entry.945527933", ((byte)age).ToString());
        form.AddField("entry.725240975", ((byte)speed).ToString());
        form.AddField("entry.43320279", ((byte)thirst).ToString());
        form.AddField("entry.32602555", ((byte)hunger).ToString());
        form.AddField("entry.1461953028", population.ToString());
        
        byte[] rawData = form.data;
        WWW www = new WWW(ANIMAL_BASE_URL, rawData);

        yield return www;
    }

    public void SendAnimalData(int simulationAge, string animalType, float age, float speed, float thirst, float hunger, int population)
    {
        StartCoroutine(PostAnimalData(simulationAge, animalType, age, speed, thirst, hunger, population));
    }
}
