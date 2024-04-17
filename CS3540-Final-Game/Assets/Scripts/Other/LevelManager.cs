
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using Cinemachine;
using System;

// behavior to manage the level and game
public class LevelManager : MonoBehaviour

{
    public GameObject healthPotionPrefab;
    public GameObject manaPotionPrefab;
    public Text gameText;
    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;
    public GameObject toolTip;

    public static GameObject player;
    public static bool isGameOver = false;
    public static bool isGameWon = false;
    public static bool isBossAwake = false;
    public static bool glovePickedUp = false;
    public static bool bootsPickedUp = false;
    public static bool shieldPickedup = false;
    // public static string savePointJSONPath = Application.dataPath + "/JSON/savePoint.json";
    public static int healthPotionAmt = 0;
    public static int manaPotionAmt = 0;
    public static int shieldAmt = 0;
    public static Transform savePoint;

    private bool gloveUIChanged = false;
    private bool bootUIChanged = false;
    private bool shieldUIChanged = false;
    private static ToolTips toolTipPanel;
    private static bool gavePotionTip = false;
    private static bool gaveBootTip = false;
    private static bool gaveGloveTip = false;
    private static bool gaveMoveTip = false;
    private static bool gaveInteractionTip = false;
    private static List<Vector3> healthPotionLocations = new List<Vector3>();
    private static List<Vector3> manaPotionLocations = new List<Vector3>();
    void Start()
    {
        toolTipPanel = toolTip.GetComponent<ToolTips>();
        isGameOver = false;
        Initialize();

        if (!gaveMoveTip)
        {
            toolTipPanel.MovementTip();
            gaveMoveTip = true;
            toolTipPanel.Invoke("InteractionTip", 1f);
            gaveInteractionTip = true;
        }

        foreach (Vector3 location in manaPotionLocations)
        {
        }
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

            if (shieldPickedup == true && !shieldUIChanged)
            {
                UpdateShieldPickUpUI();
            }
        }
    }

    public static void UpdateShieldPickUpUI()
    {
        if (!isGameOver)
        {
            UpdatePotionCountUI("ShieldPickUpIcon", shieldAmt);
        }
    }

    void Initialize()
    {
        if (!gavePotionTip && healthPotionLocations.Count == 0 && manaPotionLocations.Count == 0)
        {
            healthPotionLocations = FirstHealthPotionLocations();
            manaPotionLocations = FirstManaPotionLocations();
        }
        foreach (Vector3 location in healthPotionLocations)
        {
            Instantiate(healthPotionPrefab, location, Quaternion.identity);
        }
        foreach (Vector3 location in manaPotionLocations)
        {
            Instantiate(manaPotionPrefab, location, Quaternion.identity);
        }
        UpdateHealthPotionCountUI(healthPotionAmt);
        UpdateManaPotionCountUI(manaPotionAmt);

        print("ManaPotLocCount: " + manaPotionLocations.Count);
        print("HealthPotLocCount: " + healthPotionLocations.Count);
        LoadPlayerPrefs();
    }

    private void LoadPlayerPrefs()
    {
        int inverted = PlayerPrefs.GetInt("invertedControls", 1);
        bool isInverted = false;
        if (inverted == 1)
        {
            isInverted = true;
        }
        else if (inverted == 0)
        {
            isInverted = false;
        }
        CinemachineFreeLook freeLook = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CinemachineFreeLook>();
        freeLook.m_YAxis.m_InvertInput = isInverted;
        print(PlayerPrefs.GetFloat("volume", 100));
        freeLook.m_XAxis.m_MaxSpeed = 50 * PlayerPrefs.GetFloat("mouseSensitivity", 5);
        freeLook.m_YAxis.m_MaxSpeed = PlayerPrefs.GetFloat("mouseSensitivity", 5) * .4f;
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 5) / 10;


    }

    private static List<Vector3> FirstHealthPotionLocations()
    {
        List<Vector3> healthPotionLocations = new List<Vector3>
        {
            new Vector3(-10.80f, 32.79f, -52.83f),
            new Vector3(-10.80f, 32.79f, -53.83f),
            new Vector3(-50f, 51f, -71f),
            new Vector3(-34.4f, 54.47f, -101f)
        };
        return healthPotionLocations;
    }

    private static List<Vector3> FirstManaPotionLocations()
    {
        List<Vector3> healthPotionLocations = new List<Vector3>
        {
            new Vector3(-10.88f, 32.52f, -55.55f),
            new Vector3(-10.86f, 32.52f, -54.83f),
            new Vector3(-50f, 51f, -72f),
            new Vector3(-34.8f, 54.5f, -102f),

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

    // display the tooltip explaining how to use the potions
    public static void PotionPopup()
    {
        if (!gavePotionTip)
        {
            toolTipPanel.PotionTip();
            gavePotionTip = true;
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
            if (!gaveGloveTip) // display the tooltip explaining how to use the gloves
            {
                toolTipPanel.SuperAttack();
                gaveGloveTip = true;
            }
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
            if (!gaveBootTip) // display the tooltip explaining how to use the boots
            {
                toolTipPanel.SuperJump();
                gaveBootTip = true;
            }
        }
    }

    // reload level on lost
    public void LevelLost()
    {
        isGameOver = true;
        AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position);
        Invoke("LoadCurrentLevel", 3);
    }

    // level won
    public void LevelBeat()
    {
        isGameOver = true;
        isGameWon = true;
        AudioSource.PlayClipAtPoint(gameWonSFX, Camera.main.transform.position);
    }

    void LoadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnApplicationQuit()
    {
        print("Deleting savePoint.json file");
        // File.Delete(savePointJSONPath);
        PlayerPrefs.DeleteKey("savePoint");
    }

    public static void AddHealthPotionLocation(Vector3 potionLocation)
    {
        List<float> healthPotXs = healthPotionLocations.ConvertAll(p => p.x);
        List<float> healthPotZs = healthPotionLocations.ConvertAll(p => p.z);
        if (!healthPotXs.Contains(potionLocation.x) && !healthPotZs.Contains(potionLocation.z))
        {
            print("Adding health potion location" + healthPotionLocations.Count);
            healthPotionLocations.Add(potionLocation);
        }
        // File.WriteAllText(potionLocationsPath, JsonUtility.ToJson(potionLocations));
    }

    public static void RemoveHealthPotionLocation(Vector3 potionLocation)
    {
        List<float> healthPotXs = healthPotionLocations.ConvertAll(p => p.x);
        List<float> healthPotZs = healthPotionLocations.ConvertAll(p => p.z);
        int remIdx = healthPotionLocations.FindIndex(p => p.x == potionLocation.x && p.z == potionLocation.z);
        if (remIdx != -1)
        {
            healthPotionLocations.RemoveAt(remIdx);
        }
        // File.WriteAllText(potionLocationsPath, JsonUtility.ToJson(potionLocations));
    }

    public static List<Vector3> GetHealthPotionLocations()
    {
        return healthPotionLocations;
    }
    public static void AddManaPotionLocation(Vector3 potionLocation)
    {
        List<float> manaPotXs = manaPotionLocations.ConvertAll(p => p.x);
        List<float> manaPotZs = manaPotionLocations.ConvertAll(p => p.z);
        if (!manaPotXs.Contains(potionLocation.x) && !manaPotZs.Contains(potionLocation.z))
        {
            manaPotionLocations.Add(potionLocation);
        }
    }

    public static void RemoveManaPotionLocation(Vector3 potionLocation)
    {
        List<float> manaPotXs = manaPotionLocations.ConvertAll(p => p.x);
        List<float> manaPotZs = manaPotionLocations.ConvertAll(p => p.z);
        int remIdx = manaPotionLocations.FindIndex(p => p.x == potionLocation.x && p.z == potionLocation.z);
        if (remIdx != -1)
        {
            manaPotionLocations.RemoveAt(remIdx);
        }
    }

    public static List<Vector3> GetManaPotionLocations()
    {
        return manaPotionLocations;
    }

    public static void Reset()
    {
        print("Start Reset");
        float mouseSense = PlayerPrefs.GetFloat("mouseSensitivity", 5);
        float vol = PlayerPrefs.GetFloat("volume", 5);
        int inverted = PlayerPrefs.GetInt("invertedControls", 1);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("mouseSensitivity", mouseSense);
        PlayerPrefs.SetFloat("volume", vol);
        PlayerPrefs.SetInt("invertedControls", inverted);

        isGameOver = false;
        isGameWon = false;
        isBossAwake = false;
        glovePickedUp = false;
        bootsPickedUp = false;
        healthPotionAmt = 0;
        manaPotionAmt = 0;
        shieldAmt = 0;
        savePoint = null;
        gavePotionTip = false;
        gaveBootTip = false;
        gaveGloveTip = false;
        gaveMoveTip = false;
        gaveInteractionTip = false;
        healthPotionLocations.Clear();
        manaPotionLocations.Clear();
        print(Time.timeScale);
        print("End Reset");
    }
}

