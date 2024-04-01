using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugEnemyAI : MonoBehaviour
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
    public float wanderpointDistRange = 4f;
    // public int wanderpointAmount = 3;
    public Vector3[] wanderPoints;
    public FSMStates currentState;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float chaseRange = 8f;
    public float attackRange = 1.5f;
    public int damageAmount = 8;
    public float nextWanderpointDist = 0.75f;
    // public float attackCooldown = 1f;
    public float idleTime = 2f;
    public float lookSlerpScalar = 5f;
    public float gravity = 9.8f;
    public float minPotionFloatHeight = 0.25f;
    [Range(0, 1), Tooltip("The percentage of the attack animation duration when the enemy should shoot a projectile.")]
    public float shootPercentInAttackDuration = 0.5f;
    [Range(0, 100)]
    public int potionDropChance = 10;
    public GameObject[] potionDrops;
    public float destroyTime = 1.75f;
    public GameObject player;
    public GameObject deadVFX;
    public AudioClip deadSFX;
    public AudioClip attackSFX;
    // public AudioClip hitSFX;
    public bool isDead = false;
    Animator anim;
    CharacterController cc;
    bool canAttack = true;
    bool wentIdle = false;
    float distToPlayer;
    int currentDestIdx = 0;
    int idleIndex = 0;
    Vector3 nextDestination;
    Transform deadTransform;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        // GetWanderPoints(wanderpointAmount);
        canAttack = true;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cc.isGrounded && isDead)
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

    void UpdateIdleState()
    {
        // Debug.Log("Idle");
        anim.SetInteger("bugAnimState", 0);
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
        anim.SetInteger("bugAnimState", 1);
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
        cc.Move(transform.forward * patrolSpeed * Time.deltaTime);
    }

    void UpdateChaseState()
    {
        // Debug.Log("Chase");
        anim.SetInteger("bugAnimState", 2);
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
        cc.Move(transform.forward * chaseSpeed * Time.deltaTime);
    }

    void UpdateHitState()
    {
        // Get hit by the player, and play hit animation if health is not 0.
        // Can implement once enemy health is added.
    }

    void UpdateAttackState()
    {
        anim.SetInteger("bugAnimState", 4);
        float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
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
        if (!isDead)
        {
            anim.SetInteger("bugAnimState", 5);
            isDead = true;
            Destroy(gameObject, destroyTime);
            deadTransform = transform;
            AudioSource.PlayClipAtPoint(deadSFX, deadTransform.position);
        }
        cc.Move(Vector3.down * Time.deltaTime * gravity);
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 lookDirection = target - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
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
        if (canAttack)
        {
            float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
            canAttack = false;
            Invoke("Stab", animDuration * shootPercentInAttackDuration);
            Invoke("ResetAttack", animDuration);
        }
    }

    private void Stab()
    {
        AudioSource.PlayClipAtPoint(attackSFX, transform.position);
        if (distToPlayer <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageAmount);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
        // Debug.Log("Reset Attack");
    }

    private void DropPotion()
    {
        int dropChance = Random.Range(0, 100);
        print(dropChance);
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
        if (deadTransform)
        {
            Instantiate(deadVFX, deadTransform.position, Quaternion.Euler(-90, 0, 0));
            DropPotion();
        }
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
