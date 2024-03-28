using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackCollision : MonoBehaviour
{
    int slashDamageAmount;
    int spinDamageAmount;

    bool canDamage = true;

    void Start() {
        slashDamageAmount = GetComponentInParent<TestBossBehavior>().slashDamageAmount;
        spinDamageAmount = GetComponentInParent<TestBossBehavior>().spinDamageAmount;
    }
    
    void OnTriggerEnter(Collider other) {
        print("collided");
        String state = GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;
        print(state);
        if(other.gameObject.CompareTag("Player") && canDamage) {
            PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
            if(state == "BossSlashAttack") {
                playerHealth.TakeDamage(slashDamageAmount);
                canDamage = false;
                Invoke("ResetAttack", 2f);
            } else if(state == "BossSpinAttack") {
                playerHealth.TakeDamage(spinDamageAmount);
                canDamage = false;
                Invoke("ResetAttack", 2f);
            }
        }
    }

    void ResetAttack() {
        canDamage = true;
    }
}
