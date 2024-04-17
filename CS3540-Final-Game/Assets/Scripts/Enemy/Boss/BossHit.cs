using UnityEngine;

// behavior for boss being hit
public class BossHit : MonoBehaviour
{
    PlayerFSMController playerFSM;
    GameObject player;
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");

        playerFSM = player.GetComponent<PlayerFSMController>();

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && playerFSM.IsAttacking())
        {
            transform.parent.parent.GetComponent<BossEnemyAI>().TakeDamage(other.GetComponent<WeaponDamage>().damageAmount);
        }
    }
}
