using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotionBehavior : MonoBehaviour
{
    public static int manaAmt = 20;
    private Vector3 startPosition;
    private bool isPickedUp = false;

    void Start()
    {
        startPosition = transform.position;
        LevelManager.AddManaPotionLocation(startPosition);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            isPickedUp = true;
            LevelManager.manaPotionAmt++;
            LevelManager.PotionPopup();
            LevelManager.UpdateManaPotionCountUI(LevelManager.manaPotionAmt);
            LevelManager.RemoveManaPotionLocation(startPosition);
        }
    }
}
