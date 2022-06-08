using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    private const string startButtonName = "Start Button";
    private const string restartButtonName = "Restart Button";
    private const string panelName = "Panel";
    
    private const string startTextName = "Start Text";
    private const string winTextName = "Win Text";
    private const string loseTextName = "Lose Text";
    
    private GameObject startButton, restartButton, panel, startText, winText, loseText;

    void Start()
    {
        foreach (Transform t in transform) {
            switch (t.gameObject.name) {
                case startButtonName:
                    startButton = t.gameObject;
                    break;
                case restartButtonName:
                    restartButton = t.gameObject;
                    break;
                case panelName:
                    panel = t.gameObject;
                    break;
                case startTextName:
                    startText = t.gameObject;
                    break;
                case winTextName:
                    winText = t.gameObject;
                    break;
                case loseTextName:
                    loseText = t.gameObject;
                    break;
            }
        }
    }

    public void ToggleStartGame() {
        startButton.SetActive(false);
        restartButton.SetActive(false);
        panel.SetActive(false);
        startText.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);
    }

    public void ToggleWin() {
        startButton.SetActive(false);
        restartButton.SetActive(true);
        panel.SetActive(true);
        startText.SetActive(false);
        winText.SetActive(true);
        loseText.SetActive(false);
    }

    public void ToggleLose() {
        ToggleWin();
        winText.SetActive(false);
        loseText.SetActive(true);
    }
}
