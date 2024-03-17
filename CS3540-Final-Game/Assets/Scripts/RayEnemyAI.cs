using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Hit,
        Dead
    }
    public float wanderpointDistRange = 5f;
    // public int wanderpointAmount = 3;
    public Vector3[] wanderPoints;
    public FSMStates currentState;
    public float speed = 4f;
    public float chaseRange = 9f;
    public float attackRange = 4f;
    public float nextWanderpointDist = 0.75f;
    public float attackCooldown = 2f;
    public float idleTime = 2.5f;
    public float lookSlerpScalar = 5f;
    public float gravity = 9.8f;
    public float minPotionFloatHeight = 0.25f;
    [Range(0, 1), Tooltip("The percentage of the attack animation duration when the enemy should shoot a projectile.")]
    public float shootPercentInAttackDuration = 0.5f;
    [Range(0, 100)]
    public int potionDropChance = 10;
    public GameObject[] potionDrops;
    public float destroyTime = 2.75f;
    public GameObject[] projectiles;
    public GameObject player;
    public GameObject deadVFX;
    public AudioClip deadSFX;
    public AudioClip heavyAttackSFX;
    public AudioClip lightAttackSFX;
    // public AudioClip hitSFX;
    public Transform rayMouth;
    public bool isDead = false;
    Animator anim;
    CharacterController cc;
    bool canShoot = true;
    bool wentIdle = false;
    float distToPlayer;
    float attackDelay;
    int currentDestIdx = 0;
    int idleIndex = 0;
    Vector3 nextDestination;
    Transform deadTransform;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        // GetWanderPoints(wanderpointAmount);
        canShoot = true;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cc.isGrounded)
        {
            cc.Move(Vector3.down * Time.deltaTime * gravity);
        }
        distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        switch (currentState)
        {
            case FSMStates.Idle:
                UpdateIdleState();
                break;
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Hit:
                UpdateHitState();
                break;
            case FSMStates.Dead:
                UpdateDeadState();
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            // Debug.Log("Weapon Hit!");
            // Die on hit, will implement enemy health later
            currentState = FSMStates.Dead;
        }
    }
    void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        rayMouth = Utility.FindChildTransformWithTag(gameObject, "RayMouth");
        GetPresetWanderPoints();
        currentState = FSMStates.Patrol;
        FindNextPoint();
    }

    // Randomness would be nice, but have to ensure the wanderpoints wouldn't be too close together
    // private void GetWanderPoints(int amount)
    // {
    //     wanderPoints = new Vector3[amount];
    //     Vector3 currentPos = transform.position;
    //     for (int i = 0; i < amount; i++)
    //     {
    //         Vector3 wanderPoint = new Vector3(Random.Range(currentPos.x - wanderpointDistRange, currentPos.x + wanderpointDistRange), currentPos.y, Random.Range(currentPos.z - wanderpointDistRange, currentPos.z + wanderpointDistRange));
    //         Debug.Log("Wander Point: " + wanderPoint);
    //         wanderPoints[i] = wanderPoint;
    //     }
    //     Debug.Log("Amount: " + wanderPoints.Length);
    // }

    // Hardcoded wanderpoints for now - 3 total
    private void GetPresetWanderPoints()
    {
        wanderPoints = new Vector3[3];
        Vector3 curPos = transform.position;
        Vector3 point1 = curPos + new Vector3(wanderpointDistRange, 0, 0);
        Vector3 point2 = curPos + new Vector3(0, 0, wanderpointDistRange);
        Vector3 point3 = curPos + new Vector3(-1 * wanderpointDistRange / 2, 0, -1 * wanderpointDistRange / 2);
        wanderPoints[0] = point1;
        wanderPoints[1] = point2;
        wanderPoints[2] = point3;
    }

    private void SetPatrolState()
    {
        if (currentState == FSMStates.Idle)
        {
            FindNextPoint();
            currentState = FSMStates.Patrol;
            wentIdle = false;
        }

    }

    public void StartAttackDelay()
    {
        StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine()
    {
        // Stop the animation
        anim.enabled = false;

        // Wait for the delay
        yield return new WaitForSeconds(attackDelay);

        // Restart the animation
        anim.enabled = true;
    }

    void UpdateIdleState()
    {
        // Debug.Log("Idle");
        // Debug.Log(currentDestIdx);
        anim.SetInteger("rayAnimState", 0);
        if (!wentIdle)
        {
            wentIdle = true;
            Invoke("SetPatrolState", idleTime);
        }
        if (distToPlayer <= chaseRange)
        {
            currentState = FSMStates.Chase;
        }
    }

    void UpdatePatrolState()
    {
        // Debug.Log("Patrol - " + currentDestIdx);
        anim.SetInteger("rayAnimState", 1);
        if (distToPlayer <= chaseRange)
        {
            currentState = FSMStates.Chase;
        }
        else if (Vector3.Distance(transform.position, nextDestination) <= nextWanderpointDist)
        {
            if (currentDestIdx == idleIndex)
            {
                currentState = FSMStates.Idle;
            }
            else
            {
                // Debug.Log("Find Next Point");
                FindNextPoint();
            }
        }
        FaceTarget(nextDestination);
        cc.Move(transform.forward * speed * Time.deltaTime);
    }

    void UpdateChaseState()
    {
        // Debug.Log("Chase");
        anim.SetInteger("rayAnimState", 2);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange)
        {
            currentState = FSMStates.Attack;
        }
        else if (distToPlayer > chaseRange)
        {
            FindNextPoint();
            currentState = FSMStates.Patrol;
        }
        FaceTarget(nextDestination);
        cc.Move(transform.forward * speed * Time.deltaTime);
    }

    void UpdateHitState()
    {
        // Get hit by the player, and play hit animation if health is not 0.
        // Can implement once enemy health is added.
    }

    void UpdateAttackState()
    {
        anim.SetInteger("rayAnimState", 4);
        float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        // Ensure we don't have a negative attack delay
        attackDelay = Mathf.Max(0, attackCooldown - animDuration);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange)
        {
            currentState = FSMStates.Attack;
        }
        else if (distToPlayer > attackRange && distToPlayer <= chaseRange)
        {
            currentState = FSMStates.Chase;
        }
        else if (distToPlayer > chaseRange)
        {
            currentState = FSMStates.Patrol;
        }
        FaceTarget(nextDestination);
        EnemyAttack();
    }

    void UpdateDeadState()
    {
        // Debug.Log("Dead");
        anim.SetInteger("rayAnimState", 5);
        isDead = true;
        Destroy(gameObject, destroyTime);
        deadTransform = transform;
        AudioSource.PlayClipAtPoint(deadSFX, deadTransform.position);
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSlerpScalar);
    }

    void FindNextPoint()
    {
        if (currentDestIdx == 0)
        {
            idleIndex = Random.Range(0, wanderPoints.Length);
        }
        nextDestination = wanderPoints[currentDestIdx];
        currentDestIdx = (currentDestIdx + 1) % wanderPoints.Length;
    }

    void EnemyAttack()
    {
        if (canShoot)
        {
            float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
            canShoot = false;
            Invoke("ShootProjectile", animDuration * shootPercentInAttackDuration);
            Invoke("ResetAttack", attackCooldown);
        }
    }

    private void ShootProjectile()
    {
        if (!isDead)
        {
            Debug.Log("Shoot");
            Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            int projectileIdx = Random.Range(0, projectiles.Length);
            AudioClip attackSFX = projectileIdx == 0 ? heavyAttackSFX : lightAttackSFX;
            GameObject projectile = Instantiate(projectiles[projectileIdx], rayMouth.position, rotation);
            projectile.transform.SetParent(GameObject.FindGameObjectWithTag("ProjectileParent").transform);
            AudioSource.PlayClipAtPoint(attackSFX, rayMouth.position);
        }
    }

    private void ResetAttack()
    {
        canShoot = true;
        // Debug.Log("Reset Attack");
    }

    private void DropPotion()
    {
        int dropChance = Random.Range(0, 100);
        if (dropChance < potionDropChance)
        {
            GameObject potion = potionDrops[Random.Range(0, potionDrops.Length)];
            // Make sure the potion floats above the ground
            Vector3 dropPos = deadTransform.position;
            dropPos.y += minPotionFloatHeight;
            GameObject potObj = Instantiate(potion, dropPos, Quaternion.identity);
            potObj.transform.SetParent(GameObject.FindGameObjectWithTag("PotionParent").transform);
        }
    }

    private void OnDestroy()
    {
        Instantiate(deadVFX, deadTransform.position, Quaternion.Euler(-90, 0, 0));
        DropPotion();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        foreach (Vector3 point in wanderPoints)
        {
            Gizmos.DrawSphere(point, .5f);
            Gizmos.DrawWireSphere(point, nextWanderpointDist);
        }
    }
}
