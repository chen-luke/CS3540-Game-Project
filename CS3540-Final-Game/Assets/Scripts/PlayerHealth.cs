
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;

    //public AudioClip deadSFX;
    public AudioClip drinkPotionSFX;
    public Slider healthBar;
    int currentHealth;
    int maxHealth = 100;


    public static bool isDead = false;
    private string HP_POTION_AMT_ICON = "HpPotionIcon";
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthBar.value = currentHealth;
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
            healthBar.value = currentHealth;
            LevelManager.hpPotionAmt --;

            AudioSource.PlayClipAtPoint(drinkPotionSFX, Camera.main.transform.position);

            FindObjectOfType<LevelManager>().UpdatePotionCountUI(HP_POTION_AMT_ICON, LevelManager.hpPotionAmt);
        }

    }

    public void TakeDamage(int damageAmount) {
        if (currentHealth > 0) {
            currentHealth -= damageAmount;
            healthBar.value = currentHealth;
        }
        if (currentHealth <=0) {
            PlayerDies();
        }

        Debug.Log("Current health: " + currentHealth);
    }

    void PlayerDies() {
        Debug.Log("Player is dead");
        //AudioSource.PlayClipAtPoint(deadSFX, transform.position);
        transform.Rotate(-90, 0, 0, Space.Self);
        isDead = true;
    }


}
