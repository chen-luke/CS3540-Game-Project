using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionBehavior : MonoBehaviour
{
    public static int healthAmt = 20;
    private Vector3 startPosition;
    private bool isPickedUp = false;
    void Start()
    {
        startPosition = transform.position;
        LevelManager.AddHealthPotionLocation(startPosition);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            isPickedUp = true;
            LevelManager.healthPotionAmt++;
            LevelManager.UpdateHealthPotionCountUI(LevelManager.healthPotionAmt);
            LevelManager.RemoveHealthPotionLocation(startPosition);
        }
    }
}
