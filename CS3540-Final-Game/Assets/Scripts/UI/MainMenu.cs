using UnityEngine;
using UnityEngine.SceneManagement;

// behavior for the main menu scene
public class MainMenu : MonoBehaviour
{
    public void StartGame() {
        Time.timeScale = 1;
        LevelManager.Reset();
        Invoke("InvokeStart", 1.2f);
    }

    public void ExitGame() {
        Application.Quit();
    }

    void InvokeStart() {
        print("Build Index: " + SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
