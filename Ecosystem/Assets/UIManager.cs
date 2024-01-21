using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    public void DidClick(string sceneName)
    {
        Debug.Log(sceneName);
        gameManager.SwitchScene(sceneName);
    }
}
