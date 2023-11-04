using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendInfoToSheets : MonoBehaviour
{
    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLScQczyRS2A9Wl-xGJxcH2lL1fYEmrtj64ppIKL8xpXjBU_a8w/formResponse";
    IEnumerator PostAnimalData(string animalType, float lifetime, float speed)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.704130423", animalType);
        form.AddField("entry.945527933", lifetime.ToString());
        form.AddField("entry.725240975", speed.ToString());
        
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);

        yield return www;
    }
    public void SendAnimalData()
    {
        StartCoroutine(PostAnimalData("Animal", 2.5f, 3f));
    }

    void Start()
    {
        SendAnimalData();
    }
}
