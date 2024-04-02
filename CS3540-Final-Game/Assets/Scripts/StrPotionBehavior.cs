
using UnityEngine;

public class StrPotionBehavior : PotionBehavior
{
//     // The Tag for Stamina potion amount UI
//     public static int manaAmt = 20;
//     public AudioClip potionPickupSFX;
//     private bool soundPlayed = false;
//     private static bool isPickedUp = false;
//     protected override void Start()
//     {
//         if (isPickedUp)
//         {
//             gameObject.SetActive(false);
//         }
//         else
//         {
//             base.Start();
//             UpdatePotionCountUI(STR_POTION_AMT_ICON, LevelManager.strPotionAmt);
//         }
//     }

//     // Update is called once per frame
//     protected override void Update()
//     {
//         base.Update();
//     }

//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             LevelManager.strPotionAmt++;
//             UpdatePotionCountUI(STR_POTION_AMT_ICON, LevelManager.strPotionAmt);
//             if (!soundPlayed)
//             {
//                 AudioSource.PlayClipAtPoint(potionPickupSFX, Camera.main.transform.position);
//                 soundPlayed = true;
//             }
//             isPickedUp = true;
//             Destroy(gameObject);

//         }
//     }
}
