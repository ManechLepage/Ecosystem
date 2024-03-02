using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSceneManager : MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        Debug.Log("Switched scene !");
        GameManager.instance.SwitchScene(sceneName);
    }
}
