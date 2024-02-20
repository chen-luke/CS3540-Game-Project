using UnityEngine;


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
        if (other.CompareTag("Player")) {
                    LevelManager.hpPotionAmt++;
        UpdatePotionCountUI(HP_POTION_AMT_ICON, LevelManager.hpPotionAmt);
        Destroy(gameObject);
        Debug.Log("Added " + healAmount + " HP points to the player!");
        }

    }

}
