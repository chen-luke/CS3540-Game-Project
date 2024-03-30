using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SavePointColliderbehavior : MonoBehaviour
{
    public Transform respawnPoint;
    
    void OnTriggerEnter(Collider other)
    {
        print(Application.dataPath);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Setting respawn point");
            SavePointJSON();
        }
    }

    void SavePointJSON()
    {
        // Save the respawn point to a JSON file
        string jsonStr = JsonUtility.ToJson(respawnPoint.position);
        File.WriteAllText(LevelManager.savePointJSONPath, jsonStr);
    }
}
