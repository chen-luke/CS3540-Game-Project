
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour

{
    public static GameObject player;
    public Text gameText;

    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;
    public static bool isGameOver = false;

    public static bool glovePickedUp = false;

    public static bool bootsPickedUp = false;

    public static int hpPotionAmt = 0;
    public static int strPotionAmt = 0;
    public string nextLevel;
    public static Transform savePoint;
    private bool gloveUIChanged = false;
    
    private bool bootUIChanged = false;
    private void Awake()
    
    {
        GameObject[] islands = GameObject.FindGameObjectsWithTag("Island");
        foreach (var isl in islands)
        {
            DontDestroyOnLoad(isl);
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isGameOver = false;
        if (savePoint == null)
        {
            Debug.Log("SP IS NULL");
            savePoint = GameObject.FindGameObjectWithTag("GameStartSpawnPoint").transform;
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        Debug.Log("savePoint: " + savePoint.position);
        // player.transform.position = savePoint.position;
        player.GetComponent<PlayerController>().SetPosition(savePoint);
        Debug.Log("Player's position after setting it: " + player.transform.position);
    }

    void Update()
    {
        if (!isGameOver)
        {
            if (glovePickedUp == true && !gloveUIChanged)
            {
                UpdateGlovePickUpUI();
            }

            if (bootsPickedUp == true && !bootUIChanged)
            {
                UpdateBootPickUpUI();
            }

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

    public void UpdateGlovePickUpUI()
    {
        GameObject gloveIconUI = GameObject.FindGameObjectWithTag("GlovePickUpIcon");
        gloveIconUI.GetComponent<Image>().color = Color.green;
        gloveUIChanged = true;
    }

    public static void SetRespawnPoint(Transform newRespawnPoint)
    {
        Debug.Log("Setting new respawn point to " + newRespawnPoint.position);
        savePoint = newRespawnPoint;
    }

    public void UpdateBootPickUpUI()
    {
        GameObject bootIconUI = GameObject.FindGameObjectWithTag("BootsPickUpIcon");
        bootIconUI.GetComponent<Image>().color = Color.green;
        bootUIChanged = true;
    }

    // The below code might not be implemented due to our current design, 
    // but for now we are leaving it for protential future changes.
    // 

    public void LevelLost()
    {
        isGameOver = true;

        // gameText.text = "GAME OVER!";

        // gameText.gameObject.SetActive(true);

        // Camera.main.GetComponent<AudioSource>().pitch = 1;

        // AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position);


        Invoke("LoadCurrentLevel", 2);
    }

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

    void LoadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public static void InitializePlayer(GameObject newPlayer)
    {
        Debug.Log("Initializing player at " + savePoint.position);
        player = newPlayer;
        player.GetComponent<PlayerController>().SetPosition(savePoint);
        Debug.Log("Player's position after setting it: " + player.transform.position);
    }


}

