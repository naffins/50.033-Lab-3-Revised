using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    
    private const string UIName = "UI";

    private UIController uIController;

    void Start()
    {
        Time.timeScale = 0F;
        uIController = GameObject.Find(UIName).GetComponent<UIController>();
    }

    public void ToggleStartGame() {
        Time.timeScale = 1F;
        uIController.ToggleStartGame();
    }

    public void ToggleWin() {
        Time.timeScale = 0F;
        uIController.ToggleWin();
    }

    public void ToggleLose() {
        Time.timeScale = 0F;
        uIController.ToggleLose();
    }

    public void ToggleRestart() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
