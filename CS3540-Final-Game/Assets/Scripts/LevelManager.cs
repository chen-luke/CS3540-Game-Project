
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour

{
    public static GameObject player;
    public GameObject healthPotionPrefab;
    public GameObject manaPotionPrefab;
    public Text gameText;
    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;
    public static bool isGameOver = false;

    public static bool glovePickedUp = false;

    public static bool bootsPickedUp = false;
    public static string savePointJSONPath = Application.dataPath + "/JSON/savePoint.json";
    public static int healthPotionAmt = 0;
    public static int manaPotionAmt = 0;
    public string nextLevel;
    public static Transform savePoint;
    private bool gloveUIChanged = false;

    private bool bootUIChanged = false;
    private static List<Vector3> healthPotionLocations = FirstHealthPotionLocations();
    private static List<Vector3> manaPotionLocations = FirstManaPotionLocations();

    void Start()
    {
        isGameOver = false;
        // healthPotionAmt = 0;
        // manaPotionAmt = 0;
        Initialize();
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

    void Initialize() {
        foreach(Vector3 location in healthPotionLocations) {
            Instantiate(healthPotionPrefab, location, Quaternion.identity);
        }
        foreach(Vector3 location in manaPotionLocations) {
            Instantiate(manaPotionPrefab, location, Quaternion.identity);
        }
        UpdateHealthPotionCountUI(healthPotionAmt);
        UpdateManaPotionCountUI(manaPotionAmt);
    }

    private static List<Vector3> FirstHealthPotionLocations() {
        List<Vector3> healthPotionLocations = new List<Vector3>
        {
            new Vector3(-4.86f, 32.79f, -56.46f),
            new Vector3(-4.86f, 32.79f, -60.02f),
            new Vector3(-4.86f, 32.79f, -57.97f),
            new Vector3(-3.79f, 32.79f, -56.46f)
        };
        return healthPotionLocations;
    }

    private static List<Vector3> FirstManaPotionLocations() {
        List<Vector3> healthPotionLocations = new List<Vector3>
        {
            new Vector3(-10.88f, 32.52f, -55.55f),
            new Vector3(-10.86f, 32.52f, -54.83f),
        };
        return healthPotionLocations;
    }

    private static void UpdatePotionCountUI(string type, int amt)
    {
        GameObject potionCountString = GameObject.FindGameObjectWithTag(type);
        if (potionCountString != null)
        {
            Text potionCount1stChar = potionCountString.transform.GetChild(0).GetComponent<Text>();
            Text potionCount2ndChar = potionCountString.transform.GetChild(1).GetComponent<Text>();

            int potionCount = amt;
            amt = Mathf.Clamp(amt, 0, 99);
            potionCount1stChar.text = potionCount.ToString("D2").Substring(0, 1);
            potionCount2ndChar.text = potionCount.ToString("D2").Substring(1, 1);

            if (amt < 10)
            {
                potionCount1stChar.text = "0";
            }

        }
    }

    public static void UpdateManaPotionCountUI(int amt)
    {
        if (!isGameOver)
        {
            UpdatePotionCountUI("ManaPotionIcon", amt);
        }
    }

    public static void UpdateHealthPotionCountUI(int amt)
    {
        if (!isGameOver)
        {
            UpdatePotionCountUI("HealthPotionIcon", amt);
        }
    }

    public void UpdateGlovePickUpUI()
    {
        if (!isGameOver)
        {
            GameObject gloveIconUI = GameObject.FindGameObjectWithTag("GlovePickUpIcon");
            gloveIconUI.GetComponent<Image>().color = Color.green;
            gloveUIChanged = true;
        }
    }

    public static void SetRespawnPoint(Transform newRespawnPoint)
    {
        Debug.Log("Setting new respawn point to " + newRespawnPoint.position);
        savePoint = newRespawnPoint;
    }

    public void UpdateBootPickUpUI()
    {
        if (!isGameOver)
        {
            GameObject bootIconUI = GameObject.FindGameObjectWithTag("BootsPickUpIcon");
            bootIconUI.GetComponent<Image>().color = Color.green;
            bootUIChanged = true;
        }
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


        Invoke("LoadCurrentLevel", 3);
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

    void OnApplicationQuit()
    {
        print("Deleting savePoint.json file");
        File.Delete(savePointJSONPath);
    }

    public static void AddHealthPotionLocation(Vector3 potionLocation)
    {
        if (!healthPotionLocations.Contains(potionLocation))
        {
            healthPotionLocations.Add(potionLocation);
        }
        // File.WriteAllText(potionLocationsPath, JsonUtility.ToJson(potionLocations));
    }

    public static void RemoveHealthPotionLocation(Vector3 potionLocation)
    {
        healthPotionLocations.Remove(potionLocation);
        // File.WriteAllText(potionLocationsPath, JsonUtility.ToJson(potionLocations));
    }

    public static List<Vector3> GetHealthPotionLocations()
    {
        return healthPotionLocations;
    }
    public static void AddManaPotionLocation(Vector3 potionLocation)
    {
        if (!manaPotionLocations.Contains(potionLocation))
        {
            manaPotionLocations.Add(potionLocation);
        }
    }

    public static void RemoveManaPotionLocation(Vector3 potionLocation)
    {
        manaPotionLocations.Remove(potionLocation);
    }

    public static List<Vector3> GetManaPotionLocations()
    {
        return manaPotionLocations;
    }
}

