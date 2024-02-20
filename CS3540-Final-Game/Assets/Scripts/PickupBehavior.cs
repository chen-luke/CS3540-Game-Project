using UnityEngine;

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
