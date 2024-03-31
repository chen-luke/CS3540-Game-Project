using UnityEngine;


public class HpPotionBehavior : PotionBehavior
{
    // public static int healAmount = 20;
    // public AudioClip potionPickupSFX;

    // private bool soundPlayed = false;
    // protected override void Start()
    // {
    //     base.Start();
    //     UpdatePotionCountUI(STR_POTION_AMT_ICON, LevelManager.strPotionAmt);
    // }

    // // Update is called once per frame
    // protected override void Update()
    // {
    //     base.Update();
    // }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         LevelManager.hpPotionAmt++;
    //         UpdatePotionCountUI(HP_POTION_AMT_ICON, LevelManager.hpPotionAmt);
    //         if (!soundPlayed)
    //         {
    //             AudioSource.PlayClipAtPoint(potionPickupSFX, Camera.main.transform.position);
    //             soundPlayed = true;
    //         }
    //         Destroy(gameObject);
    //         Debug.Log("Added " + healAmount + " HP points to the player!");
    //     }

    // }

}
