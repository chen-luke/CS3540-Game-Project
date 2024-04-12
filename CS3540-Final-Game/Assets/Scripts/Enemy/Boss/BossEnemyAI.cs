using UnityEngine;

// behavior for boss 
public class BossEnemyAI : MonoBehaviour
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
        Retreat,
        Defeated
    }

    public GameObject player;
    public BossFSMStates currentState;
    public Transform exhaustPipe;
    public Transform eyes;
    public CharacterController cc;
    public GameObject deadVFX;

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
    public float searchTime = 6f;

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
    public AudioClip engineWakeupSFX;
    public AudioClip engineShutOffSFX;


    // Local fields
    Animator anim;
    Vector3 nextDestination;
    bool wentIdle;
    float distToPlayer;
    bool canAttack = true;
    bool isAttacking = false;
    BossFSMStates nextAttack;
    float attackRange;
    float deathAnimationTimer = 4f;
    EnemyHealth health;
    AudioSource attackSound;

    float searchDuration = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // if the boss is dead and hasn't finished its death animation, the current state is defeated
        if (health.isDead && deathAnimationTimer > 0f && currentState != BossFSMStates.ActiveIdle)
        {
            currentState = BossFSMStates.Defeated;
        }
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
            case BossFSMStates.Defeated:
                UpdateDefeatState();
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
        health = gameObject.GetComponent<EnemyHealth>();
        attackSound = gameObject.AddComponent<AudioSource>();
        SelectAttack();
        FindNextPoint();
    }

    public void TakeDamage(int amount)
    {
        health.TakeDamage(amount);
    }

    void UpdateIdleState()
    {
        // set it to boss waking state if it is awake
        if (LevelManager.isBossAwake)
        {
            anim.SetInteger("BossAnimState", 5);
            Invoke("BossWake", 1f);
        }
    }

    void UpdateActiveIdleState()
    {
        anim.SetInteger("BossAnimState", 0);
    }

    void UpdatePatrolState()
    {
        anim.SetInteger("BossAnimState", 6);
        if (distToPlayer <= chaseRange && IsPlayerInFOV())
        {
            currentState = BossFSMStates.Chase;
        }
        FindNextPoint();
        FaceTarget(nextDestination);
        cc.Move(transform.forward * patrolSpeed * Time.deltaTime);
    }

    void UpdateChaseState()
    {
        if (IsPlayerInFOV())
        {
            searchDuration = 0f;
            anim.SetInteger("BossAnimState", 6);
            // get player position, but don't let boss go below min height
            nextDestination = new Vector3(player.transform.position.x, Mathf.Max(player.transform.position.y, minHeight), player.transform.position.z);
            if (distToPlayer - attackRange < .5 && distToPlayer - attackRange > -0.5 && CanAttack()) // within attack range?
            {
                currentState = nextAttack;
            }
            else if (distToPlayer > chaseRange) // outside of chase range?
            {
                FindNextPoint();
                currentState = BossFSMStates.Patrol;
            }
            else if (distToPlayer - attackRange < 0) // too close to player? back up
            {
                FaceTarget(nextDestination);
                cc.Move(-transform.forward * chaseSpeed * Time.deltaTime);
            }
            else
            {
                FaceTarget(nextDestination);
                cc.Move(transform.forward * chaseSpeed * Time.deltaTime);
            }
        } else {
            if(searchDuration < searchTime) {
                // print(searchDuration);
                searchDuration += Time.deltaTime;
                Vector3 rotatedVector = Quaternion.AngleAxis(30, Vector3.up) * transform.forward;
                //transform.rotation = Vector3.Slerp(transform.forward, rotatedVector, Time.deltaTime * lookSlerpScalar);
                FaceTarget(rotatedVector + transform.position);
                //cc.Move(transform.forward * chaseSpeed * Time.deltaTime);
            } else {
                FindNextPoint();
                currentState = BossFSMStates.Patrol;
                searchDuration = 0;
                print(currentState);
            }
            
        }
    }

    void UpdateAttackState(int animState)
    {
        if (!isAttacking)
        {
            if (distToPlayer - attackRange < .5 && distToPlayer - attackRange > -0.5 && canAttack) // within attack range?
            {
                nextDestination = player.transform.position;
                FaceTarget(nextDestination);
                anim.SetInteger("BossAnimState", animState);
                isAttacking = true;
                Attack(animState);
            }
            else if (distToPlayer <= chaseRange) // not within attack range, but within chase range?
            {
                currentState = BossFSMStates.Chase;
            }
            else if (distToPlayer > chaseRange) // outside of chase range?
            {
                currentState = BossFSMStates.Patrol;
            }
        }
    }

    void UpdateRetreatState()
    {
        anim.SetInteger("BossAnimState", 6);
        float retreatedDist = Vector3.Distance(transform.position, player.transform.position);
        if (retreatedDist >= retreatDistance) // is boss far enough away from the player?
        {
            if (distToPlayer - attackRange < .5 && distToPlayer - attackRange > -0.5) // within attack range?
            {
                currentState = nextAttack;
            }
            else if (distToPlayer <= chaseRange) // outside attack range but within chase range?
            {
                currentState = BossFSMStates.Chase;
            }

            else if (distToPlayer > chaseRange) // outside chase range?
            {
                FindNextPoint();
                currentState = BossFSMStates.Patrol;
            }
        }
        else // still too close to player, back up more
        {
            currentState = BossFSMStates.Retreat;
            FaceTarget(player.transform.position);
            cc.Move(-transform.forward * patrolSpeed * Time.deltaTime);
        }
    }

    void UpdateDefeatState()
    {
        anim.SetInteger("BossAnimState", 7);
        if (deathAnimationTimer <= 0f) // has it been long enough for the defeated animation to play through?
        {
            Defeat();
            currentState = BossFSMStates.ActiveIdle;

        }
        else
        {
            deathAnimationTimer -= Time.deltaTime;
        }

    }

    // void FindNextPoint()
    // {
    //     if (InWanderCircle()) // if boss position is already on the wander circle, move to next position on it
    //     {
    //         float dist = Vector3.Distance(transform.position, nextDestination); 
    //         if (dist > -.05 && dist < .05) // at destination yet?
    //         {
    //             nextDestination = GetNextPointInCircle();
    //         }
    //     }
    //     else // if boss position not on wander circle, find closest point in circle to boss current location
    //     {
    //         nextDestination = FindClosestPointInCircle();
    //     }
    // }

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

    // is boss position located on the wander circle?
    bool InWanderCircle()
    {
        float dist = Vector3.Distance(transform.position, wanderCenter.position);
        bool inCircle = dist - wanderRadius > -2 && dist - wanderRadius < 2;
        return inCircle;
    }

    // find closest point to boss current position that is on the wander circle
    Vector3 FindClosestPointInCircle()
    {
        Vector3 dist = transform.position - wanderCenter.position;
        Vector3 origVec = wanderCenter.forward * wanderRadius;
        float angle = Vector3.Angle(origVec, dist);
        float x = wanderCenter.position.x + (wanderRadius * Mathf.Cos(angle));
        float z = wanderCenter.position.z + (wanderRadius * Mathf.Sin(angle));
        return new Vector3(x, wanderCenter.position.y, z);
    }

    // find position of point on wander circle rotated 50* from current boss position
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
        // if boss is low enough, don't look down any more
        if (transform.position.y - minHeight < 2)
        {
            direction.y = 0;
        }
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSlerpScalar);
    }

    // randomly select the type of attack boss will do next
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
            // call appropriate attack method after the attack animation's duration has passed
            switch (currentState)
            {
                case BossFSMStates.SpinAttack:
                    attackMethod = "SpinAttack";
                    animDuration = 2f;
                    attackSound.clip = spinAttackSFX;
                    attackSound.Play();
                    break;
                case BossFSMStates.SlashAttack:
                    attackMethod = "SlashAttack";
                    attackSound.clip = slashAttackSFX;
                    attackSound.Play();
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
        isAttacking = false;
        if (!health.isDead)
        {
            // attackSound.clip = spinAttackSFX;
            // attackSound.Play();
            Retreat();
        }
    }

    private void SlashAttack()
    {
        isAttacking = false;
        if (!health.isDead)
        {
            // attackSound.clip = slashAttackSFX;
            // attackSound.Play();
            Retreat();
        }
    }

    private void FireAttack()
    {
        if (!health.isDead)
        {
            Quaternion rotation = Quaternion.LookRotation(transform.forward);
            Instantiate(fireProjectile, exhaustPipe.transform.position, rotation);
            attackSound.clip = fireAttackSFX;
            attackSound.Play();
            Invoke("Retreat", anim.GetCurrentAnimatorStateInfo(0).length - 1.5f);
        }
        else
        {
            isAttacking = false;
        }
    }

    private void Retreat()
    {
        isAttacking = false;
        currentState = BossFSMStates.Retreat;
        SelectAttack();
        Invoke("ResetAttack", attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void BossWake()
    {
        Invoke("StartEngine", 2f);
        currentState = BossFSMStates.Patrol;
    }

    // play engine audio
    private void StartEngine()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }

    // check to see if there are any obstacles between the boss and the player
    private bool CanAttack()
    {
        RaycastHit hit;
        if (Physics.BoxCast(transform.position + 3 * transform.forward, transform.localScale * 0.5f, transform.forward, out hit, Quaternion.identity, attackRange - 3))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPlayerInFOV()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.transform.position - eyes.position;
        Vector3.Angle(directionToPlayer, transform.forward);
        if (Vector3.Angle(directionToPlayer, transform.forward) <= 100)
        {
            if (Physics.Raycast(eyes.position, directionToPlayer, out hit, chaseRange))
            {
                // print(hit.collider);
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // visually show that boss is defeated and invoke game beat state
    void Defeat()
    {
        print("defeat");
        gameObject.GetComponent<AudioSource>().Pause();
        AudioSource.PlayClipAtPoint(engineShutOffSFX, transform.position);
        Instantiate(deadVFX, transform.position, transform.rotation);
        FindObjectOfType<LevelManager>().Invoke("LevelBeat", 3f);
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
        //Draw a Ray forward from GameObject toward the hit
        Gizmos.DrawRay(transform.position + 3 * transform.forward, transform.forward * (attackRange - 3));
        //Draw a cube that extends to where the hit exists
        Gizmos.DrawWireCube(transform.position + 3 * transform.forward + transform.forward * (attackRange - 3), transform.localScale);
        Gizmos.DrawRay(transform.position, player.transform.position - transform.position);
    }

    // not sure if we may need later

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         if (currentState == BossFSMStates.SlashAttack)
    //         {
    //             PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
    //             playerHealth.TakeDamage(slashDamageAmount);
    //         }
    //         else if (currentState == BossFSMStates.SpinAttack)
    //         {
    //             PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
    //             playerHealth.TakeDamage(spinDamageAmount);
    //         }
    //     }
    // }
}
