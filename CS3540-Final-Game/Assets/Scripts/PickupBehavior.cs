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

    }

    void Update()
    {
        if (LevelManager.isGameOver)
        {
            // If game over, reset all potions.
            LevelManager.strPotionAmt = 0;
            LevelManager.hpPotionAmt = 0;
        }

    }

}
