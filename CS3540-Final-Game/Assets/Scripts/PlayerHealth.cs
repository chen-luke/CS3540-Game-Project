
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public AudioClip drinkPotionSFX;
    public Slider healthBar;
    int currentHealth;
    int maxHealth = 100;
    public static bool isDead = false;
    
    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        currentHealth = startingHealth;
        healthBar.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isDead)
        {
            AddHealth(HealthPotionBehavior.healthAmt);
        }
    }

    public void AddHealth(int healthAmt)
    {

        if (currentHealth < maxHealth && LevelManager.healthPotionAmt > 0)
        {
            currentHealth += healthAmt;
            healthBar.value = currentHealth;
            LevelManager.healthPotionAmt--;

            AudioSource.PlayClipAtPoint(drinkPotionSFX, Camera.main.transform.position);

            LevelManager.UpdateHealthPotionCountUI(LevelManager.healthPotionAmt);
        }

    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damageAmount;
            healthBar.value = currentHealth;
        }
        if (currentHealth <= 0)
        {
            PlayerDies();
        }

        Debug.Log("Current health: " + currentHealth);
    }

    public void PlayerDies()
    {
        //AudioSource.PlayClipAtPoint(deadSFX, transform.position);
        if (!isDead)
        {
            isDead = true;
            FindObjectOfType<LevelManager>().LevelLost();
        }
    }


}
