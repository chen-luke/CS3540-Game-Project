using System;
using UnityEngine;

// behavior for collision of boss with player during boss attack
public class BossAttackCollision : MonoBehaviour
{
    int slashDamageAmount; // amount of damage done by slash attack
    int spinDamageAmount; // amount of damage done by spin attack
    bool canDamage = true; // whether the collision can damage. set to false if the current attack has already done damage to player

    void Start()
    {
        slashDamageAmount = GetComponentInParent<BossEnemyAI>().slashDamageAmount;
        spinDamageAmount = GetComponentInParent<BossEnemyAI>().spinDamageAmount;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!gameObject.GetComponentInParent<EnemyHealth>().isDead)
        {
            string state = GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;
            // check collision is with player
            if (other.gameObject.CompareTag("Player") && canDamage)
            {
                PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
                // apply appropriate damage amount based on the current attack state. If no attack state, don't apply damage
                if (state == "BossSlashAttack")
                {
                    playerHealth.TakeDamage(slashDamageAmount);
                    canDamage = false;
                    Invoke("ResetAttack", 2f);
                }
                else if (state == "BossSpinAttack")
                {
                    playerHealth.TakeDamage(spinDamageAmount);
                    canDamage = false;
                    Invoke("ResetAttack", 2f);
                }
            }
        }

    }

    void ResetAttack()
    {
        canDamage = true;
    }
}
