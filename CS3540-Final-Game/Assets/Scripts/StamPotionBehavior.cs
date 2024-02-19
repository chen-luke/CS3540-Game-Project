using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StamPotionBehavior : PotionBehavior
{
    // The Tag for Stamina potion amount UI
    public static int healAmount = 20;
    protected override void Start()
    {
        base.Start();
        UpdatePotionCountUI(STAMINA_POTION_AMT_ICON, LevelManager.staminaPotionAmt);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    void OnTriggerEnter(Collider other)
    {
        LevelManager.staminaPotionAmt ++;
        UpdatePotionCountUI(STAMINA_POTION_AMT_ICON, LevelManager.staminaPotionAmt);
        Debug.Log("Added " + healAmount + " stamina points to the player!");

        Destroy(gameObject);
    }
}
