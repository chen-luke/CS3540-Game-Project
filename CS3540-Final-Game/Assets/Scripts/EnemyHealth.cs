using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    // Start is called before the first frame update
    public int startHealth = 100;
    public Slider healthSlider;

    public bool isDead = false;
    private int currentHealth;
    void Start()
    {
        currentHealth = startHealth;
        healthSlider.value = currentHealth;
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
                isDead = true;
            }

        }
    }
}
