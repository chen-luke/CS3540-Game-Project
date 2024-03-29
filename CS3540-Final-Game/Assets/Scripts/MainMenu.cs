using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame() {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Invoke("InvokeStart", 1.2f);
    }

    public void ExitGame() {
        Application.Quit();
    }

    void InvokeStart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
   
}
