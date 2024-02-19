using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpPotionBehavior : PotionBehavior
{
    public static int healAmount = 20;

    protected override void Start()
    {
        base.Start();
        UpdatePotionCountUI(HP_POTION_AMT_ICON, LevelManager.hpPotionAmt);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    void OnTriggerEnter(Collider other)
    {
        LevelManager.hpPotionAmt++;
        UpdatePotionCountUI(HP_POTION_AMT_ICON, LevelManager.hpPotionAmt);
        Destroy(gameObject);
        Debug.Log("Added " + healAmount + " HP points to the player!");
    }

}
