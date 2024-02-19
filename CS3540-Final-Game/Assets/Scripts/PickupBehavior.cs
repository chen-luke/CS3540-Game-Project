using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PickupBehavior : MonoBehaviour
{
    private Material m;

    [SerializeField]
    private float rotationSpeed;


    public AudioClip pickupSFX;
    void Start()
    {
        // potion count default 1?
        //pickupCount++;
        // 
    }

    void Update()
    {
        if (LevelManager.isGameOver)
        {
            // If game over, reset all potions.
            LevelManager.staminaPotionAmt = 0;
            LevelManager.hpPotionAmt = 0;
        }

    }

    private void OnDestroy()
    {
        if (!LevelManager.isGameOver)
        {
            // Minus potion
            // Gain Effect
            // pickupCount--;

            // if (pickupCount <= 0)
            // {
            //     FindObjectOfType<LevelManager>().LevelBeat();
            // }
        }
    }

    public void ChangePotionAmt(string method, string potionType)
    {
        
    }


}
