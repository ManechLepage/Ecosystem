using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSceneManager : MonoBehaviour
{
    public string sceneName;
    public bool triggersWhenEnter = false;
    
    public void SwitchScene()
    {
        Debug.Log("Switched scene !");
        GameManager.instance.SwitchScene(sceneName);
    }

    void Update()
    {
        if (triggersWhenEnter && Input.GetKeyDown(KeyCode.Return))
            SwitchScene();
    }
}
