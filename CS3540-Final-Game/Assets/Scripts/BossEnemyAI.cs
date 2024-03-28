using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossEnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Idle,
        Waking,
        ActiveIdle,
        Patrol,
        Chase,
        SlashAttack,
        SpinAttack,
        FireAttack
    }

    // wander point fields
    public float wanderpointDistRange = 5f;
    // public int wanderpointAmount = 3;
    //public Vector3[] wanderPoints;
    public float wanderRadius;

    public Transform wanderCenter;

    // fsm field
    public FSMStates currentState;

    // movement
    public float speed = 4f;
    public float chaseRange = 9f;
    public float attackRange = 4f;
    //public float nextWanderpointDist = 0.75f;
    public float attackCooldown = 2f;
    public float idleTime = 2.5f;
    public float lookSlerpScalar = 5f;
    //public float gravity = 9.8f;

    //[Range(0, 1), Tooltip("The percentage of the attack animation duration when the enemy should shoot a projectile.")]
    //public float shootPercentInAttackDuration = 0.5f;
    public GameObject fireProjectile;
    public GameObject player;
    // public GameObject deadVFX;
    // public AudioClip deadSFX;
    public AudioClip spinAttackSFX;
    public AudioClip slashAttackSFX;
    public AudioClip fireAttackSFX;
    public Transform exhaustPipe;
    public bool isDead = false;

    public int slashDamageAmount = 10;
    public int spinDamageAmount = 5;
    Animator anim;
    CharacterController cc;
    bool canAttack = true;
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
        canAttack = true;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {

        distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        switch (currentState)
        {
            case FSMStates.Idle:
                UpdateIdleState();
                break;
            case FSMStates.ActiveIdle:
                UpdateActiveIdleState();
                break;
            case FSMStates.Waking:
                UpdateWakingState();
                break;
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.SpinAttack:
                UpdateSpinAttackState();
                break;
            case FSMStates.SlashAttack:
                UpdateSlashAttackState();
                break;
            case FSMStates.FireAttack:
                UpdateFireAttackState();
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            // Debug.Log("Weapon Hit!");
            // Die on hit, will implement enemy health later

        }
    }
    void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        exhaustPipe = Utility.FindChildTransformWithTag(gameObject, "ExhaustPipe");
        //GenerateWanderPoints();
        currentState = FSMStates.Patrol;
        FindNextPoint();
    }


    // // Hardcoded wanderpoints for now - 3 total
    // private void GetPresetWanderPoints()
    // {
    //     wanderPoints = new Vector3[3];
    //     Vector3 curPos = transform.position;
    //     Vector3 point1 = curPos + new Vector3(wanderpointDistRange, 0, 0);
    //     Vector3 point2 = curPos + new Vector3(0, 0, wanderpointDistRange);
    //     Vector3 point3 = curPos + new Vector3(-1 * wanderpointDistRange / 2, 0, -1 * wanderpointDistRange / 2);
    //     wanderPoints[0] = point1;
    //     wanderPoints[1] = point2;
    //     wanderPoints[2] = point3;
    // }

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
        anim.SetInteger("BossAnimState", 0);
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

    void UpdateActiveIdleState()
    {
        // Debug.Log("Idle");
        // Debug.Log(currentDestIdx);
        anim.SetInteger("BossAnimState", 0);
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
        anim.SetInteger("BossAnimState", 1);
        if (distToPlayer <= chaseRange)
        {
            currentState = FSMStates.Chase;
        }
        else if (InWanderCircle())
        {
            // continue along circle
        }
        FaceTarget(nextDestination);
        cc.Move(transform.forward * speed * Time.deltaTime);
    }

    void UpdateChaseState()
    {
        // Debug.Log("Chase");
        anim.SetInteger("BossAnimState", 2);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange)
        {
            SelectAttack();
            //currentState = FSMStates.Attack;
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

    void UpdateSpinAttackState()
    {
        anim.SetInteger("BossAnimState", 4);
        float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        // Ensure we don't have a negative attack delay
        attackDelay = Mathf.Max(0, attackCooldown - animDuration);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange)
        {
            currentState = FSMStates.SpinAttack;
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
        SpinAttack();
    }

    void UpdateSlashAttackState()
    {
        anim.SetInteger("BossAnimState", 4);
        float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        // Ensure we don't have a negative attack delay
        attackDelay = Mathf.Max(0, attackCooldown - animDuration);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange)
        {
            currentState = FSMStates.SlashAttack;
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
        SlashAttack();
    }

    void UpdateFireAttackState()
    {
        anim.SetInteger("BossAnimState", 4);
        float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        // Ensure we don't have a negative attack delay
        attackDelay = Mathf.Max(0, attackCooldown - animDuration);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange)
        {
            currentState = FSMStates.FireAttack;
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
        FireAttack();
    }

    void UpdateWakingState()
    {
        anim.SetInteger("BossAnimState", 4);
        float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        // Ensure we don't have a negative attack delay
        attackDelay = Mathf.Max(0, attackCooldown - animDuration);
        nextDestination = player.transform.position;
        if (distToPlayer <= attackRange)
        {
            currentState = FSMStates.FireAttack;
        }
        else if (distToPlayer > attackRange && distToPlayer <= chaseRange)
        {
            currentState = FSMStates.Chase;
        }
        else if (distToPlayer > chaseRange)
        {
            currentState = FSMStates.Patrol;
        }
    }

    // void UpdateDeadState()
    // {
    //     // Debug.Log("Dead");
    //     anim.SetInteger("BossAnimState", 5);
    //     isDead = true;
    //     Destroy(gameObject, destroyTime);
    //     deadTransform = transform;
    //     AudioSource.PlayClipAtPoint(deadSFX, deadTransform.position);
    // }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSlerpScalar);
    }

    void FindNextPoint()
    {
        if (!InWanderCircle())
        {
            FindClosestPointInCircle();
        }
        //nextDestination = point;
    }

    void SelectAttack()
    {
        int attackType = Random.Range(0, 2);
        switch (attackType)
        {
            case 0:
                currentState = FSMStates.SpinAttack;
                break;
            case 1:
                currentState = FSMStates.SlashAttack;
                break;
            case 2:
                currentState = FSMStates.FireAttack;
                break;
        }

    }

      void UpdateAttackState()
    {
        
        float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        // Ensure we don't have a negative attack delay
        attackDelay = Mathf.Max(0, attackCooldown - animDuration);
        //nextDestination = player.transform.position;
        if (distToPlayer <= attackRange)
        {
            //currentState = FSMStates.SpinAttack;
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
        Attack();
    }

    void Attack()
    {
        if (canAttack)
        {
            float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
            canAttack = false;
            string attackMethod = "";
            switch (currentState)
            {
                case FSMStates.SpinAttack:
                    attackMethod = "SpinAttack";
                    break;
                case FSMStates.SlashAttack:
                    attackMethod = "SlashAttack";
                    break;
                case FSMStates.FireAttack:
                    attackMethod = "FireAttack";
                    break;
            }
            Invoke(attackMethod, animDuration);
            Invoke("ResetAttack", attackCooldown);
        }
    }
    void SpinAttack()
    {
        if (!isDead)
        {
            anim.SetInteger("BossAnimState", 2);
            AudioSource.PlayClipAtPoint(spinAttackSFX, transform.position);
            if (distToPlayer <= attackRange)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                playerHealth.TakeDamage(spinDamageAmount);
            }
        }
    }

    void SlashAttack()
    {
        if (!isDead)
        {
            AudioSource.PlayClipAtPoint(slashAttackSFX, transform.position);
            if (distToPlayer <= attackRange)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                playerHealth.TakeDamage(slashDamageAmount);
            }
        }
    }

    void FireAttack()
    {
        if (!isDead)
        {
            
            Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            //int projectileIdx = Random.Range(0, projectiles.Length);
            //AudioClip attackSFX = projectileIdx == 0 ? heavyAttackSFX : lightAttackSFX;
            GameObject projectile = Instantiate(fireProjectile, exhaustPipe.position, rotation);
            projectile.transform.SetParent(GameObject.FindGameObjectWithTag("ProjectileParent").transform);
            AudioSource.PlayClipAtPoint(fireAttackSFX, exhaustPipe.position);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
        // Debug.Log("Reset Attack");
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wanderCenter.position, wanderRadius);
    }

    bool InWanderCircle() {
        float dist = Vector3.Distance(transform.position, wanderCenter.position);
        return dist > wanderRadius - .05 || dist < wanderRadius + .05;
    }

    void FindClosestPointInCircle(){
        Vector3 dist = transform.position - wanderCenter.position;
        Vector3 origVec = wanderCenter.forward*wanderRadius;
        float angle = Vector3.Angle(origVec, dist);
        float x = wanderCenter.position.x + (wanderRadius*Mathf.Cos(angle));
        float z = wanderCenter.position.z + (wanderRadius*Mathf.Sin(angle));
        Vector3 closestPoint = new Vector3(x, transform.position.y, z);
    }
}
