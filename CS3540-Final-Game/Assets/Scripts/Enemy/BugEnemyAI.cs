using UnityEngine;

public class BugEnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }
    public FSMStates currentState;
    public GameObject player;
    public GameObject deadVFX;
    public float destroyTime = 1.75f; // how long after death to destroy object

    // Patrol Zone
    [Header("Patrol Zone Settings")]
    public float wanderpointDistRange = 4f;
    public Vector3[] wanderPoints;
    public float nextWanderpointDist = 0.75f;

    // Movement Settings
    [Header("Movement Settings")]
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float chaseRange = 8f;
    public float attackRange = 1.5f;
    public float idleTime = 2f;
    public float lookSlerpScalar = 5f;
    public float gravity = 9.8f;

    // Attack Settings
    [Header("Attack Settings")]
    public int damageAmount = 8;
    [Range(0, 1), Tooltip("The percentage of the attack animation duration when the enemy should shoot a projectile.")]
    public float shootPercentInAttackDuration = 0.5f;

    // Potion Settings
    [Header("Potion Settings")]
    public float minPotionFloatHeight = 0.25f;
    [Range(0, 100)]
    public int potionDropChance = 10;
    public GameObject[] potionDrops;

    // Audio clips
    [Header("Audio Clips")]
    public AudioClip deadSFX;
    public AudioClip attackSFX;
    public AudioClip[] hitSFX;

    // local fields
    Animator anim;
    CharacterController cc;
    bool canAttack = true;
    bool wentIdle = false;
    float distToPlayer;
    int currentDestIdx = 0;
    int idleIndex = 0;
    Vector3 nextDestination;
    Transform deadTransform;
    EnemyHealth enemyHealth;
    EnemySight enemySight;
    PlayerFSMController playerFSM;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        canAttack = true;
        enemyHealth.isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHealth.isDead)
        {
            currentState = FSMStates.Dead;
        }
        if (!cc.isGrounded && enemyHealth.isDead) // make enemy fall to ground if dead
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
            case FSMStates.Dead:
                UpdateDeadState();
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon") && playerFSM.IsAttacking())
        {
            int sfxIndex = Random.Range(0, hitSFX.Length);
            AudioSource.PlayClipAtPoint(hitSFX[sfxIndex], transform.position, .5f);
            enemyHealth.TakeDamage(collision.gameObject.GetComponent<WeaponDamage>().damageAmount);
        }

    }
    void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerFSM = player.GetComponent<PlayerFSMController>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        GetPresetWanderPoints();
        currentState = FSMStates.Patrol;
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
        enemySight = gameObject.GetComponent<EnemySight>();
        FindNextPoint();
    }

    // generate wanderpoint locations
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
        anim.SetInteger("bugAnimState", 1);
        if (enemySight.SeePlayer()) // Do we see the player?
        {
            currentState = FSMStates.Chase;
        }
        else if (Vector3.Distance(transform.position, nextDestination) <= nextWanderpointDist) // if enemy at the wanderpoint?
        {
            if (currentDestIdx == idleIndex)
            {
                currentState = FSMStates.Idle;
            }
            else
            {
                FindNextPoint();
            }
        }
        FaceTarget(nextDestination);
        cc.Move(transform.forward * patrolSpeed * Time.deltaTime);
    }

    void UpdateChaseState()
    {
        anim.SetInteger("bugAnimState", 2);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange) // is player within attack range?
        {
            currentState = FSMStates.Attack;
        }
        else if (distToPlayer > chaseRange) // is player outside of chase range?
        {
            FindNextPoint();
            currentState = FSMStates.Patrol;
        }
        FaceTarget(nextDestination);
        cc.Move(transform.forward * chaseSpeed * Time.deltaTime);
    }

    void UpdateAttackState()
    {
        anim.SetInteger("bugAnimState", 4);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange) // is player within attack range?
        {
            currentState = FSMStates.Attack;
        }
        else if (distToPlayer > attackRange && distToPlayer <= chaseRange) // is player outside attack range but inside chase range?
        {
            currentState = FSMStates.Chase;
        }
        else if (distToPlayer > chaseRange) // is player outside of chase range?
        {
            currentState = FSMStates.Patrol;
        }
        FaceTarget(nextDestination);
        EnemyAttack();
    }

    void UpdateDeadState()
    {
        if (enemyHealth.isDead)
        {
            anim.SetInteger("bugAnimState", 5);
            Destroy(gameObject, destroyTime);
            deadTransform = transform;
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
        AudioSource.PlayClipAtPoint(attackSFX, transform.position, .5f);
        if (distToPlayer <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageAmount);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
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
