using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    private Material m;

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
