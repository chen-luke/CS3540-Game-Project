using UnityEngine;


public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float walkSpeed = 5.0f;
    public float sprintSpeedScalar = 2.0f;
    public float jumpForceScalar = 2.0f;

    public float superJumpForceScalar = 3f;
    public float gravity = 9.81f;
    public float airControl = 0.75f;
    CharacterController cc;
    private float moveSpeed;

    private float jumpAmount;
    Vector3 input, moveDirection;

    Animator m_Animator;

    private float minHeight = 26f;

    public enum FSMStates
    {
        Idle, RStrafe, LStrafe, Running, Attack, HeavyAttack, Jump, Sprint, Death
    }

    public FSMStates currentState;


    void Start()
    {
        currentState = FSMStates.Idle;
        cc = GetComponent<CharacterController>();
        m_Animator = gameObject.GetComponent<Animator>();
        if (LevelManager.savePoint)
        {
            SetPosition(LevelManager.savePoint);
        }
        PlayerHealth.isDead = false;

    }

    void Update()
    {
        if (!PlayerHealth.isDead)
        {
            HandleState();
            MovePlayer();

            // Check if the player fell below minHeight
            if (transform.position.y < minHeight)
            {
                PlayerHealth.isDead = true;
                FindObjectOfType<LevelManager>().LevelLost();
            }
        }
    }

    void HandleState()
    {
        switch (currentState)
        {
            case FSMStates.Idle:
                IdleAnimation();
                break;
            case FSMStates.Running:
                RunAnimation();
                break;
            case FSMStates.Sprint:
                SprintAnimation();
                break;
            case FSMStates.Jump:
                JumpAnimation();
                break;
            case FSMStates.Attack:
                AttackAnimation();
                break;
            case FSMStates.HeavyAttack:
                HeavyAttackAnimation();
                break;
            case FSMStates.Death:
                DeathAnimation();
                break;
        }
    }

    void MovePlayer()
    {
        // Get movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;
        float jumpAmount = Input.GetKey(KeyCode.LeftShift) && cc.isGrounded && LevelManager.bootsPickedUp ? superJumpForceScalar : jumpForceScalar;


        // Update moveSpeed based on input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = walkSpeed * sprintSpeedScalar;
            currentState = FSMStates.Sprint;
            
        }
        else
        {
            moveSpeed = walkSpeed;
        }


        // Handle jumping
        if (Input.GetButtonDown("Jump") && currentState != FSMStates.Jump )
        {
            moveDirection.y = Mathf.Sqrt(2 * jumpAmount * gravity);
            currentState = FSMStates.Jump;
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            currentState = FSMStates.Attack;
        }
        else if (Input.GetKey(KeyCode.F) && LevelManager.glovePickedUp)
        {
            currentState = FSMStates.HeavyAttack;
        }

        // Update moveDirection
        moveDirection = input * moveSpeed;
        cc.Move(moveDirection * Time.deltaTime);
    }

    private void IdleAnimation()
    {
        m_Animator.SetInteger("animState", 0);

        // Check for transitions to other states
        if (PlayerHealth.isDead)
        {
            currentState = FSMStates.Death;
        }
        else if (Input.GetButton("Jump"))
        {
            currentState = FSMStates.Jump;
        }
        else if (input.magnitude > 0)
        {
            currentState = FSMStates.Running;
        }
    }

    private void RunAnimation()
    {
        m_Animator.SetInteger("animState", 3);

        // Check for transitions to other states
        if (PlayerHealth.isDead)
        {
            currentState = FSMStates.Death;
        }
        else if (Input.GetButton("Jump"))
        {
            currentState = FSMStates.Jump;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            currentState = FSMStates.Sprint;
        }
        else if (input.magnitude == 0)
        {
            currentState = FSMStates.Idle;
        }
    }

    private void SprintAnimation()
    {
        m_Animator.SetInteger("animState", 5);

        // Check for transitions to other states
        if (PlayerHealth.isDead)
        {
            currentState = FSMStates.Death;
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            currentState = FSMStates.Running;
        }
        else if (Input.GetButton("Jump"))
        {
            currentState = FSMStates.Jump;
        }
    }

    private void JumpAnimation()
    {
        m_Animator.SetInteger("animState", 4);

        // Check for transitions to other states
        if (PlayerHealth.isDead)
        {
            currentState = FSMStates.Death;
        }

        if (currentState == FSMStates.Jump && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !m_Animator.IsInTransition(0))
        {
            print("Jump animation finished playing!");

            if (input.magnitude > 0)
            {
                currentState = FSMStates.Running;
            }
            else
            {
                currentState = FSMStates.Idle;
                //print("idle transition from jump");
            }
        }

    }

    private void AttackAnimation()
    {
        m_Animator.SetInteger("animState", 6);
        if (currentState == FSMStates.Attack && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !m_Animator.IsInTransition(0))
        {
            if (input.magnitude > 0)
            {
                currentState = FSMStates.Running;
            }
            else
            {
                currentState = FSMStates.Idle;
            }
        }
    }

    private void HeavyAttackAnimation()
    {
        m_Animator.SetInteger("animState", 7);
    }
    private void DeathAnimation()
    {
        m_Animator.SetInteger("animState", 8);
    }

    public void SetPosition(Transform newTransform)
    {
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
        Physics.SyncTransforms();
    }
}


