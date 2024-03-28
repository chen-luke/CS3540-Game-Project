using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TestBossBehavior : MonoBehaviour
{

    public enum BossFSMStates
    {
        Asleep,
        ActiveIdle,
        Patrol,
        Chase,
        SlashAttack,
        SpinAttack,
        FireAttack,
        Retreat
    }

    public GameObject player;

    public BossFSMStates currentState;
    public bool isDead = false;

    public Transform exhaustPipe;

    // Patrol Zone
    [Header("Patrol Zone Settings")]
    public float wanderRadius;
    public Transform wanderCenter;

    // Movement Controls
    [Header("Movement Controls")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 2f;
    public float lookSlerpScalar = .5f;
    public float idleTime = 2.5f;
    public float minHeight = 54f;
    public float retreatDistance = 6f;

    // Range Settings
    [Header("Range Settings")]
    public float chaseRange = 9f;
    public float spinAttackRange = 5f;
    public float slashAttackRange = 3f;
    public float fireAttackRange = 8f;
    public float damageRange = 1f;

    // Attack Settings
    [Header("Attack Settings")]
    public int slashDamageAmount = 10;
    public int spinDamageAmount = 5;
    public float attackCooldown = 2f;
    public GameObject fireProjectile;

    // Audio Clips
    [Header("Audio Clips")]
    public AudioClip spinAttackSFX;
    public AudioClip slashAttackSFX;
    public AudioClip fireAttackSFX;


    // Local fields
    Animator anim;
    Vector3 nextDestination;
    bool wentIdle;
    float distToPlayer;
    bool canAttack = true;
    float attackDelay;
    bool isAttacking = false;

    BossFSMStates nextAttack;
    float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();

    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        switch (currentState)
        {
            case BossFSMStates.Asleep:
                UpdateIdleState();
                break;
            case BossFSMStates.ActiveIdle:
                UpdateActiveIdleState();
                break;
            case BossFSMStates.Patrol:
                UpdatePatrolState();
                break;
            case BossFSMStates.Chase:
                UpdateChaseState();
                break;
            case BossFSMStates.SpinAttack:
                UpdateAttackState(2);
                break;
            case BossFSMStates.SlashAttack:
                UpdateAttackState(3);
                break;
            case BossFSMStates.FireAttack:
                UpdateAttackState(4);
                break;
            case BossFSMStates.Retreat:
                UpdateRetreatState();
                break;
        }
    }

    void Initialize()
    {
        anim = GetComponentInChildren<Animator>();
        wentIdle = false;
        player = GameObject.FindGameObjectWithTag("Player");
        distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        currentState = BossFSMStates.Asleep;
        SelectAttack();
        FindNextPoint();
    }

    void UpdateIdleState()
    {
        if(LevelManager.isBossAwake) {
            anim.SetInteger("BossAnimState", 7);
            Invoke("BossWake", 1f);
        }
    }

    void UpdateActiveIdleState()
    {
        anim.SetInteger("BossAnimState", 0);
        if (!wentIdle)
        {
            wentIdle = true;
            Invoke("SetPatrolState", idleTime);
        }
        if (distToPlayer <= chaseRange)
        {
            currentState = BossFSMStates.Chase;
        }
    }

    void UpdatePatrolState()
    {
        anim.SetInteger("BossAnimState", 6);
        if (distToPlayer <= chaseRange)
        {
            currentState = BossFSMStates.Chase;
        }
        FindNextPoint();
        FaceTarget(nextDestination);
        transform.position = Vector3.MoveTowards(transform.position, nextDestination, patrolSpeed * Time.deltaTime);
    }

    void UpdateChaseState()
    {
        // Debug.Log("Chase");
        anim.SetInteger("BossAnimState", 6);
        nextDestination = new Vector3(player.transform.position.x, Mathf.Max(player.transform.position.x, minHeight), player.transform.position.z);
        if (distToPlayer - attackRange < .5 && distToPlayer - attackRange > -0.5)
        {
            currentState = nextAttack;
        }
        else if (distToPlayer > chaseRange)
        {
            FindNextPoint();
            currentState = BossFSMStates.Patrol;
        } else if(distToPlayer - attackRange < 0) {
            FaceTarget(nextDestination);
            transform.position = Vector3.Lerp(transform.position, transform.position - new Vector3(0, 0, 5), chaseSpeed * Time.deltaTime);
        }
        else
        {
            FaceTarget(nextDestination);
            transform.position = Vector3.MoveTowards(transform.position, nextDestination, chaseSpeed * Time.deltaTime);
        }


        //cc.Move(transform.forward * speed * Time.deltaTime);
    }

    void UpdateAttackState(int animState)
    {
        
        
        if (distToPlayer - attackRange < .5 && distToPlayer - attackRange > -0.5 && !isAttacking)
        {
            nextDestination = player.transform.position;
            FaceTarget(nextDestination);
            anim.SetInteger("BossAnimState", animState);
            Attack(animState);
        }
        else if (distToPlayer > attackRange && distToPlayer <= chaseRange && !isAttacking)
        {
            currentState = BossFSMStates.Chase;
        }
        else if (distToPlayer > chaseRange && !isAttacking)
        {
            currentState = BossFSMStates.Patrol;
        }
    }

    void UpdateRetreatState()
    {
        //Debug.Log("Retreat");
        anim.SetInteger("BossAnimState", 6);
        float retreatedDist = Vector3.Distance(transform.position, nextDestination);
        if (retreatedDist < 0.5f && retreatedDist > -0.5f)
        {
            if (distToPlayer - attackRange < .5 && distToPlayer - attackRange > -0.5)
            {
                currentState = nextAttack;
            }
            else if (distToPlayer > attackRange && distToPlayer <= chaseRange)
            {
                currentState = BossFSMStates.Chase;
            }

            else if (distToPlayer > chaseRange)
            {
                FindNextPoint();
                currentState = BossFSMStates.Patrol;
            }
        }
        else
        {
            currentState = BossFSMStates.Retreat;
            FaceTarget(player.transform.position);
            transform.position = Vector3.Lerp(transform.position, nextDestination, Time.deltaTime*2);
        }
    }

    void FindNextPoint()
    {
        if (InWanderCircle())
        {
            float dist = Vector3.Distance(transform.position, nextDestination);
            if (dist > -.05 && dist < .05)
            {
                nextDestination = GetNextPointInCircle();
            }
        }
        else
        {
            nextDestination = FindClosestPointInCircle();
        }
    }

    bool InWanderCircle()
    {
        float dist = Vector3.Distance(transform.position, wanderCenter.position);
        bool inCircle = dist - wanderRadius > -2 && dist - wanderRadius < 2;
        return inCircle;
    }

    Vector3 FindClosestPointInCircle()
    {
        Vector3 dist = transform.position - wanderCenter.position;
        Vector3 origVec = wanderCenter.forward * wanderRadius;
        float angle = Vector3.Angle(origVec, dist);
        float x = wanderCenter.position.x + (wanderRadius * Mathf.Cos(angle));
        float z = wanderCenter.position.z + (wanderRadius * Mathf.Sin(angle));
        return new Vector3(x, wanderCenter.position.y, z);
    }

    Vector3 GetNextPointInCircle()
    {
        float angle = 50;
        float x1 = transform.position.x - wanderCenter.position.x;
        float z1 = transform.position.z - wanderCenter.position.z;
        float x2 = x1 * Mathf.Cos(angle) + z1 * Mathf.Sin(angle) + wanderCenter.position.x;
        float z2 = -x1 * Mathf.Sin(angle) + z1 * Mathf.Cos(angle) + wanderCenter.position.z;
        return new Vector3(x2, wanderCenter.position.y, z2);
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSlerpScalar);
    }

    void SelectAttack()
    {
        int attackType = Random.Range(0, 3);
        switch (attackType)
        {
            case 0:
                nextAttack = BossFSMStates.SpinAttack;
                attackRange = spinAttackRange;
                break;
            case 1:
                nextAttack = BossFSMStates.SlashAttack;
                attackRange = slashAttackRange;
                break;
            case 2:
                nextAttack = BossFSMStates.FireAttack;
                attackRange = fireAttackRange;
                break;
        }
    }

    void Attack(int animState)
    {
        if (canAttack)
        {
            float animDuration = 0;
            canAttack = false;
            isAttacking = true;
            string attackMethod = "";
            switch (currentState)
            {
                case BossFSMStates.SpinAttack:
                    attackMethod = "SpinAttack";
                    animDuration = 2f;
                    break;
                case BossFSMStates.SlashAttack:
                    attackMethod = "SlashAttack";
                    animDuration = 1.5f;
                    break;
                case BossFSMStates.FireAttack:
                    attackMethod = "FireAttack";
                    animDuration = 1.5f;
                    break;
            }
            Invoke(attackMethod, animDuration);

        }
    }
    private void SpinAttack()
    {
        if (!isDead)
        {
            AudioSource.PlayClipAtPoint(spinAttackSFX, transform.position);
            Retreat();
        }
    }

    private void SlashAttack()
    {
        if (!isDead)
        {
            AudioSource.PlayClipAtPoint(slashAttackSFX, transform.position);
            Retreat();
        }
    }

    private void FireAttack()
    {
        if (!isDead)
        {

            Quaternion rotation = Quaternion.LookRotation(transform.forward);
            Instantiate(fireProjectile, exhaustPipe.transform.position, rotation);
            AudioSource.PlayClipAtPoint(fireAttackSFX, exhaustPipe.transform.position);
            Invoke("Retreat", anim.GetCurrentAnimatorStateInfo(0).length - 1.5f);
        }

    }

    private void Retreat()
    {
        isAttacking = false;
        currentState = BossFSMStates.Retreat;
        SelectAttack();
        nextDestination = new Vector3(transform.position.x, transform.position.y, transform.position.z - retreatDistance);
        Invoke("ResetAttack", attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void BossWake() {
        currentState = BossFSMStates.Patrol;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wanderCenter.position, wanderRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
    void OnTriggerEnter(Collider other) {
        print("collided");
        if(other.gameObject.CompareTag("Player")) {
            if(currentState == BossFSMStates.SlashAttack) {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                print("ouch!");
                playerHealth.TakeDamage(slashDamageAmount);
            } else if(currentState == BossFSMStates.SpinAttack) {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                print("ouch!");
                playerHealth.TakeDamage(spinDamageAmount);

            }
        }
    }
}
