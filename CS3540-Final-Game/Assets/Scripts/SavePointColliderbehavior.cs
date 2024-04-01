using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SavePointColliderbehavior : MonoBehaviour
{
    public Transform respawnPoint;
    public AudioClip savePointSFX;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !LevelManager.isGameOver && LevelManager.savePoint != respawnPoint)
        {
            Debug.Log("Setting respawn point");
            AudioSource.PlayClipAtPoint(savePointSFX, Camera.main.transform.position);
            LevelManager.SetRespawnPoint(respawnPoint);
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
