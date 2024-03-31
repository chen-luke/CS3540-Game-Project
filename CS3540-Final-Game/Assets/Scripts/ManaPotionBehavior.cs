using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotionBehavior : MonoBehaviour
{
    public static int manaAmt = 20;
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
            LevelManager.manaPotionAmt++;
            levelManager.UpdateManaPotionCountUI(LevelManager.manaPotionAmt);
            isPickedUp = true;
        }
    }
}
