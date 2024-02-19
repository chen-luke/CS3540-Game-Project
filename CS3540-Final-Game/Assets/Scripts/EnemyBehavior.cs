using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Transform player;
    // Can add roaming behavior in the future
    public float chaseSpeed = 5f;
    public int damage = 25;
    public float damageDistance = 2f;
    // Area of Engament distance
    public float aoeDistance = 15f;
    private bool canAttack = true;
    public float attackCooldown = 1f;


    // Start is called before the first frame update
    void Start()
    {
        canAttack = true;
        if (player == null)
        {
            try { player = GameObject.FindGameObjectWithTag("Player").transform; }
            catch { throw new Exception("There is no assigned player object with the designated 'Player' tag."); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerHealth.isDead)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= aoeDistance && distance > damageDistance)
            {
                transform.LookAt(player);
                transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            }
            else if (distance <= damageDistance && canAttack)
            {
                AttackPlayer();
            }
        }
    }

    void AttackPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(damage);
        canAttack = false;
        Invoke("EnableAttack", attackCooldown);
    }

    private void EnableAttack()
    {
        canAttack = true;
    }
}
