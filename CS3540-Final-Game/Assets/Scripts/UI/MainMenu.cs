using UnityEngine;
using UnityEngine.SceneManagement;

// behavior for the main menu scene
public class MainMenu : MonoBehaviour
{
    public void StartGame() {
        Invoke("InvokeStart", 1.2f);
    }

    public void ExitGame() {
        Application.Quit();
    }

    void InvokeStart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
