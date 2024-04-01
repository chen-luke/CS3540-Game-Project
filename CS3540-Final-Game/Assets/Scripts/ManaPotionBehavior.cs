using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotionBehavior : MonoBehaviour
{
    public static int manaAmt = 20;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        LevelManager.AddManaPotionLocation(startPosition);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.manaPotionAmt++;
            LevelManager.UpdateManaPotionCountUI(LevelManager.manaPotionAmt);
            LevelManager.RemoveManaPotionLocation(startPosition);
        }
    }
}
