using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionBehavior : MonoBehaviour
{
    public static int healthAmt = 20;
    private bool isPickedUp = false;
    private LevelManager levelManager;

    void Start()
    {
        if (isPickedUp)
        {
            Destroy(gameObject);
        }
        else
        {
            levelManager = FindObjectOfType<LevelManager>();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            LevelManager.healthPotionAmt++;
            levelManager.UpdateHealthPotionCountUI(LevelManager.healthPotionAmt);
            isPickedUp = true;
        }
    }
}
