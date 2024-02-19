using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;

    public AudioClip deadSFX;
    public Slider healthSlider;
    int currentHealth;
    int maxHealth = 100;

    int maxStamina = 100;

    private string HP_POTION_AMT_ICON = "HpPotionIcon";



    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            AddHealth(HpPotionBehavior.healAmount);
        }
    }

    public void AddHealth(int healthAmt) {

        if (currentHealth < maxHealth && LevelManager.hpPotionAmt > 0) {
            currentHealth += healthAmt;
            healthSlider.value = currentHealth;
            LevelManager.hpPotionAmt --;
            FindObjectOfType<LevelManager>().UpdatePotionCountUI(HP_POTION_AMT_ICON, LevelManager.hpPotionAmt);
        }
        if (currentHealth <=0) {
            PlayerDies();
        }
    }

        public void AddStamina(int staminaAmt) {

        if (currentHealth < maxStamina && LevelManager.hpPotionAmt > 0) {
            currentHealth += staminaAmt;
            healthSlider.value = currentHealth;
            LevelManager.hpPotionAmt --;
            FindObjectOfType<LevelManager>().UpdatePotionCountUI(HP_POTION_AMT_ICON, LevelManager.hpPotionAmt);
        }
        if (currentHealth <=0) {
            PlayerDies();
        }
    }
    public void TakeDamage(int damageAmount) {
        if (currentHealth > 0) {
            currentHealth -= damageAmount;
            healthSlider.value = currentHealth;
        }
        if (currentHealth <=0) {
            PlayerDies();
        }

        Debug.Log("Current health: " + currentHealth);
    }

    void PlayerDies() {
        Debug.Log("Player is dead");
        AudioSource.PlayClipAtPoint(deadSFX, transform.position);
        transform.Rotate(-90, 0, 0, Space.Self);
    }


}
