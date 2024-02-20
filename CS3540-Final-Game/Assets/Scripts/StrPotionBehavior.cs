
using UnityEngine;

public class StrPotionBehavior : PotionBehavior
{
    // The Tag for Stamina potion amount UI
    public static int healAmount = 20;
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
        if(other.CompareTag("Player")) {
        LevelManager.strPotionAmt ++;
        UpdatePotionCountUI(STR_POTION_AMT_ICON, LevelManager.strPotionAmt);
        Debug.Log("Added " + healAmount + " str points to the player!");

        Destroy(gameObject);

        }
    }
}
