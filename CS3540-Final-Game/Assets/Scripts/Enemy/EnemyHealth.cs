using UnityEngine;
using UnityEngine.UI;

// behavior for the health of an enemy
public class EnemyHealth : MonoBehaviour
{
    public int startHealth = 100;
    public Slider healthSlider;
    public AudioClip deathSFX;
    public float deathVolume = 1;
    public bool isDead = false;

    private int currentHealth;
    void Start()
    {
        currentHealth = startHealth;
        healthSlider.maxValue = startHealth;
        healthSlider.value = currentHealth;
        isDead = false;
    }
    public void TakeDamage(int damageAmount)
    {
        if (!isDead)
        {
            if (currentHealth > 0)
            {
                currentHealth -= damageAmount;
                healthSlider.value = currentHealth;
            }
            if (currentHealth <= 0)
            {
                if (deathSFX)
                {
                    AudioSource.PlayClipAtPoint(deathSFX, transform.position, deathVolume);
                }
                isDead = true;
            }

        }
    }
}
