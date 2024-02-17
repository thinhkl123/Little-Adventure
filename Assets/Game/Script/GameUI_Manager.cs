using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI_Manager : MonoBehaviour
{
    public GameManager gameManager;
    public Slider healthSlider;
    public TMPro.TextMeshProUGUI coinText;

    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameFinished;
    private enum GameUI_State
    {
        GamePlay,
        GamePause,
        GameOver,
        GameFinished
    }

    private GameUI_State gameState;

    private void Start()
    {
        SwitchToState(GameUI_State.GamePlay);
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = gameManager.playedCharacter.GetComponent<Health>().currentHealthPercentage;
        coinText.text = gameManager.playedCharacter.coin.ToString();
    }

    private void SwitchToState(GameUI_State newState)
    {
        //Exit State
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameFinished.SetActive(false);

        Time.timeScale = 1;

        //Switch State
        switch (newState)
        {
            case GameUI_State.GamePlay:
                break;
            case GameUI_State.GamePause:
                Time.timeScale = 0;
                UI_Pause.SetActive(true);
                break;
            case GameUI_State.GameFinished:
                UI_GameFinished.SetActive(true);
                break;
            case GameUI_State.GameOver:
                UI_GameOver.SetActive(true);
                break;
        }

        gameState = newState;
    }

    public void TooglePauseUI()
    {
        if (gameState == GameUI_State.GamePlay)
        {
            SwitchToState(GameUI_State.GamePause);
        }
        else if (gameState == GameUI_State.GamePause)
        {
            SwitchToState(GameUI_State.GamePlay);
        }
    }

    public void Button_ReStart()
    {
        gameManager.ReStart();
    }

    public void Button_MainMenu()
    {
        gameManager.ReturnToMainMenu();
    }

    public void ShowGameOverUI()
    {
        SwitchToState(GameUI_State.GameOver);
    }

    public void ShowGameFinishedUI()
    {
        SwitchToState(GameUI_State.GameFinished);
    }
}
