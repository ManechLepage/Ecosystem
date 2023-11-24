using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendInfoToSheets : MonoBehaviour
{
    [SerializeField]
    private string ANIMAL_BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLScQczyRS2A9Wl-xGJxcH2lL1fYEmrtj64ppIKL8xpXjBU_a8w/formResponse";
    [SerializeField]
    private string SIMULATION_BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeQ-VFUF9GO5eyfC83NmSaIoSswHiCtapYEEspHWI6H3uoTHQ/formResponse";
    
    void Start()
    {
        // StartCoroutine(PostAnimalData(1, "rabbit", 1f, 1f, 1f, 1f, 1));
        // SendSimulationData("1x1", 1, 1, 1, 1);
    }
    // IEnumerator PostAnimalData(int simulationAge, string animalType, float age, float speed, float thirst, float hunger) //, int population)
    // {
    //     WWWForm form = new WWWForm();

    //     form.AddField("entry.1681844743", simulationAge.ToString());
    //     form.AddField("entry.704130423", animalType);
    //     form.AddField("entry.945527933", ((byte)age).ToString());
    //     form.AddField("entry.725240975", ((byte)speed).ToString());
    //     form.AddField("entry.43320279", ((byte)thirst).ToString());
    //     form.AddField("entry.32602555", ((byte)hunger).ToString());
    //     form.AddField("entry.1461953028", "0"); //population.ToString());
        
    //     byte[] rawData = form.data;
    //     WWW www = new WWW(ANIMAL_BASE_URL, rawData);

    //     yield return www;
    // }

    IEnumerator PostAnimalData(string dimensions, int simulationAge, int elks, int antelopes, int coyotes)
    {
        WWWForm form = new WWWForm();

        form.AddField("entry.1681844743", dimensions);
        form.AddField("entry.704130423", simulationAge.ToString());
        form.AddField("entry.945527933", ((byte)elks).ToString());
        form.AddField("entry.725240975", ((byte)antelopes).ToString());
        form.AddField("entry.43320279", ((byte)coyotes).ToString());
        form.AddField("entry.32602555", "0");
        form.AddField("entry.1461953028", "0");
        byte[] rawData = form.data;
        WWW www = new WWW(ANIMAL_BASE_URL, rawData);

        yield return www;
    }

    IEnumerator PostSimulationData(string dimensions, int simulationAge, int elk, int antelopes, int coyotes)
    {
        WWWForm form = new WWWForm();

        form.AddField("entry.1464067088", dimensions);
        form.AddField("entry.397784314", simulationAge.ToString());
        form.AddField("entry.599672498", elk.ToString());
        form.AddField("entry.154349931", antelopes.ToString());
        form.AddField("entry.223927614", coyotes.ToString());

        byte[] rawData = form.data;
        WWW www = new WWW(SIMULATION_BASE_URL, rawData);

        yield return www;
    }

    // public void SendAnimalData(int simulationAge, string animalType, float age, float speed, float thirst, float hunger) //, int population)
    // {
    //     StartCoroutine(PostAnimalData(simulationAge, animalType, age, speed, thirst, hunger)); //, population));
    // }

    public void SendSimulationData(string dimensions, int simulationAge, int elk, int antelopes, int coyotes)
    {
        Debug.Log("SENDING SIMULATION DATA");
        StartCoroutine(PostAnimalData(dimensions, simulationAge, elk, antelopes, coyotes));
    }
}
