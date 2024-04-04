using UnityEngine;

// behavior for boss being hit
public class BossHit : MonoBehaviour
{
   void OnTriggerEnter(Collider other) {
    if(other.CompareTag("Weapon")) {
        transform.parent.parent.GetComponent<BossEnemyAI>().TakeDamage(other.GetComponent<WeaponDamage>().damageAmount);
    }
   }
}
