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
                isGamePaused = true;
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                losePanel.SetActive(true);
            }
        }
    }

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
