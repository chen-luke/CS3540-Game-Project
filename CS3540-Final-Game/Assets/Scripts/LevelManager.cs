
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour

{
    public Text gameText;

    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;
    public static bool isGameOver = false;

    public static int hpPotionAmt = 0;
    public static int strPotionAmt = 0;
    public string nextLevel;

    void Start()
    {
        isGameOver = false;

    }

    void Update()
    {
        if (!isGameOver)
        {

        }

    }

    public void UpdatePotionCountUI(string type, int amt)
    {
        GameObject potionCountString = GameObject.FindGameObjectWithTag(type);
        if (potionCountString != null)
        {
            Text potionCount1stChar = potionCountString.transform.GetChild(0).GetComponent<Text>();
            Text potionCount2ndChar = potionCountString.transform.GetChild(1).GetComponent<Text>();

            int potionCount = amt;
            potionCount1stChar.text = potionCount.ToString("D2").Substring(0, 1);
            potionCount2ndChar.text = potionCount.ToString("D2").Substring(1, 1);

            if (amt < 10)
            {
                potionCount1stChar.text = "0";
            }

        }
    }

    // The below code might not be implemented due to our current design, 
    // but for now we are leaving it for protential future changes.
    // 

    // public void LevelLost()
    // {
    //     isGameOver = true;

    //     // gameText.text = "GAME OVER!";

    //     // gameText.gameObject.SetActive(true);

    //     // Camera.main.GetComponent<AudioSource>().pitch = 1;

    //     // AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position);

    //     Invoke("LoadCurrentLevel", 2);

    // }

    // public void LevelBeat()
    // {
    //     isGameOver = true;

    //     // gameText.text = "YOU WIN!";

    //     // gameText.gameObject.SetActive(true);

    //     // Camera.main.GetComponent<AudioSource>().pitch = 2;

    //     // AudioSource.PlayClipAtPoint(gameWonSFX, Camera.main.transform.position);

    //     if (!string.IsNullOrEmpty(nextLevel))
    //     {
    //         Invoke("LoadNextLevel", 2);
    //     }

    // }
    // void LoadNextLevel()
    // {
    //     SceneManager.LoadScene(nextLevel);
    // }

    // void LoadCurrentLevel()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }

}

