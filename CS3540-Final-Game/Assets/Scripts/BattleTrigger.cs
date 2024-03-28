using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player"))
        LevelManager.isBossAwake = true;
    }
}
