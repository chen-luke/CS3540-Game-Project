using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    public int startingMana = 100;
    public int maxMana = 100;
    public Slider manaBar;
    public AudioClip drinkPotionSFX;
    int currentMana;

    void Start()
    {
        currentMana = startingMana;
        manaBar.value = currentMana;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !PlayerHealth.isDead) {
            Addstr(ManaPotionBehavior.manaAmt);
        }
    }

    public void Addstr(int manaAmt) {

        if (currentMana < maxMana && LevelManager.manaPotionAmt > 0) {
            currentMana += manaAmt;
            manaBar.value = currentMana;
            LevelManager.manaPotionAmt--;
            AudioSource.PlayClipAtPoint(drinkPotionSFX, Camera.main.transform.position);
            FindObjectOfType<LevelManager>().UpdateManaPotionCountUI(LevelManager.manaPotionAmt);
        }
    }

    public int GetCurrentMana() {
        int retMana = currentMana;
        return retMana;
    }

    public void UseMana(int manaAmt) {
        if (currentMana >= manaAmt && manaAmt > 0) {
            currentMana -= manaAmt;
            manaBar.value = currentMana;
        }
    }
}
