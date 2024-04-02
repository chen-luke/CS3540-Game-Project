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

    private int currentHealth;
    void Start()
    {
        currentHealth = startHealth;
        healthSlider.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageAmount) {
        if (currentHealth > 0) {
            currentHealth -= damageAmount;
            healthSlider.value = currentHealth;
        }
        if (currentHealth <= 0) {
            GetComponent<BugEnemyAI>().SetIsDead();
        }
    }
    void OnTriggerEnter(Collider other) {
        // if (other.CompareTag("")) {
            
        // }
    }
}
