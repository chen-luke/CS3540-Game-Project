using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WinDieMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject winPanel;
    public GameObject losePanel;

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.isGameOver)
        {
            if (LevelManager.isGameWon)
            {
                winPanel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                //ResumeGame();
            }
            else
            {
                //PauseGame();
                losePanel.SetActive(true);
            }
        }
    }

    // void PauseGame() {
    //     isGamePaused = true;
    //     Time.timeScale = 0f;
    //     messagePanel.SetActive(true);

    //     Cursor.visible = true;
    //     Cursor.lockState = CursorLockMode.None;
    //     Camera.main.GetComponent<AudioSource>().volume *= .5f;
    // }

    // public void ResumeGame() {
    //     isGamePaused = false;
    //     Time.timeScale = 1f;
    //     messagePanel.SetActive(false);

    //     Cursor.visible = false;
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Camera.main.GetComponent<AudioSource>().volume /= .5f;
    // }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        isGamePaused = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
