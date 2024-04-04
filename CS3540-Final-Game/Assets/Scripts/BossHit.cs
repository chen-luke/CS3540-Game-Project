using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHit : MonoBehaviour
{
   void OnTriggerEnter(Collider other) {
    //print(other);
    if(other.CompareTag("Weapon")) {
        transform.parent.parent.GetComponent<BossEnemyAI>().TakeDamage(other.GetComponent<WeaponDamage>().damageAmount);
    }
   }
}
