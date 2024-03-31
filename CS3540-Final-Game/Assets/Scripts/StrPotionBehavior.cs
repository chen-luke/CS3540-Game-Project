
using UnityEngine;

public class StrPotionBehavior : PotionBehavior
{
    // The Tag for Stamina potion amount UI
    public static int healAmount = 20;
    public AudioClip potionPickupSFX;
    private bool soundPlayed = false;
    protected override void Start()
    {
        base.Start();
        UpdatePotionCountUI(STR_POTION_AMT_ICON, LevelManager.strPotionAmt);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.strPotionAmt++;
            LevelManager.PotionPopup();
            UpdatePotionCountUI(STR_POTION_AMT_ICON, LevelManager.strPotionAmt);
            if (!soundPlayed)
            {
                AudioSource.PlayClipAtPoint(potionPickupSFX, Camera.main.transform.position);
                soundPlayed = true;
            }

            Destroy(gameObject);

        }
    }
}
