using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SavePointColliderbehavior : MonoBehaviour
{
    public Transform respawnPoint;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Setting respawn point");
            LevelManager.SetRespawnPoint(respawnPoint);
        }
    }
}
