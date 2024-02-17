using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character playedCharacter;
    private bool gameOver;

    public GameUI_Manager gameUI_Manager;

    private void Awake()
    {
        playedCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            gameUI_Manager.TooglePauseUI();
        }

        if (playedCharacter.curState == Character.CharacterState.Dead)
        {
            gameOver = true;
            GameOver();
        }
    }

    public void GameOver()
    {
        gameUI_Manager.ShowGameOverUI();
    }

    public void GameFinished()
    {
        gameUI_Manager.ShowGameFinishedUI();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
