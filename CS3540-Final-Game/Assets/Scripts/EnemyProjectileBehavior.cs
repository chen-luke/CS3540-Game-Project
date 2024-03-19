using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerHealth playerHealth;
    public int projectileDamage = 10;
    GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        
        transform.LookAt(player.transform);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Weapon"))
        {
            playerHealth.TakeDamage(projectileDamage);
        }
    }
}
