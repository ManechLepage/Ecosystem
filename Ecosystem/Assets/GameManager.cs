using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Create,
    EcosystemSimulation
}

public class GameManager : MonoBehaviour
{
    [Header("General Settings")]
    public static GameManager instance = null;
    public int seed;
    public GameState gameState = GameState.Menu;
    public Vector2 size = new Vector2(64, 64);

    [Header("Ecosystem Settings")]
    public EcosystemData ecosystemData;

    [Header("Other Settings")]
    public int numberOfSimulations = 1;
    public bool addWalls = true;
    public int maxSeed = 100_000;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log(sceneName);
        gameState = (GameState)System.Enum.Parse(typeof(GameState), sceneName);
    }
}
