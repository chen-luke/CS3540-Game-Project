
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

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
        if (File.Exists(LevelManager.savePointJSONPath))
        {
            SetPosition();
        }
        else
        {
            print("No save point found");
        }
        PlayerHealth.isDead = false;
        print(Vector3.Angle(Vector3.right, Vector3.forward));
        print(Vector3.Angle(Vector3.right, Vector3.left));
        print(Vector3.Angle(Vector3.right, Vector3.right));
        print(Vector3.Angle(Vector3.right, Vector3.back));
        print(Vector3.Angle(Vector3.zero, Vector3.back));

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
                IdleState();
                break;
            case FSMStates.Running:
                RunState();
                break;
            case FSMStates.Sprint:
                SprintState();
                break;
            case FSMStates.Jump:
                JumpState();
                break;
            case FSMStates.Attack:
                AttackState();
                break;
            case FSMStates.HeavyAttack:
                HeavyAttackState();
                break;
            case FSMStates.Death:
                DeathState();
                break;
        }
    }

    void MovePlayer()
    {
        // Get movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;
        // print("The current state is: " + currentState);
        jumpAmount = Input.GetKey(KeyCode.LeftShift) && LevelManager.bootsPickedUp ? superJumpForceScalar : jumpForceScalar;


        // Update moveSpeed based on input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = walkSpeed * sprintSpeedScalar;
           // currentState = FSMStates.Sprint;
            
        }
        else
        {
            moveSpeed = walkSpeed;
        }


    //    Handle jumping
        // if (Input.GetButtonDown("Jump"))
        // {
        //     print("Did this jump work?");
        //     moveDirection.y = Mathf.Sqrt(2 * jumpAmount * gravity);
        //     currentState = FSMStates.Jump;
        // }
        // else
        // {
        //     moveDirection.y -= gravity * Time.deltaTime;
        // }

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

    private void IdleState()
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

    private void RunState()
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
            moveDirection.y = Mathf.Sqrt(2 * jumpAmount * gravity);
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

    private void SprintState()
    {
        m_Animator.SetInteger("animState", 5);

        // Check for transitions to other states
        if (PlayerHealth.isDead)
        {
            currentState = FSMStates.Death;
        }
        else if (Input.GetButton("Jump"))
        {
            currentState = FSMStates.Jump;
            moveDirection.y = Mathf.Sqrt(2 * jumpAmount * gravity);
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            currentState = FSMStates.Running;
        }
    }

    private void JumpState()
    {
        m_Animator.SetInteger("animState", 4);

        // Check for transitions to other states
        if (PlayerHealth.isDead)
        {
            currentState = FSMStates.Death;
        }

        // if current state is jump and animation finished playing 
        if (currentState == FSMStates.Jump && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !m_Animator.IsInTransition(0))
        {

            moveDirection.y -= gravity * Time.deltaTime;
            
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

    private void AttackState()
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

    private void HeavyAttackState()
    {
        m_Animator.SetInteger("animState", 7);
    }
    private void DeathState()
    {
        m_Animator.SetInteger("animState", 8);
    }

    public void SetPosition()
    {
        string posStr = File.ReadAllText(LevelManager.savePointJSONPath);
        print(posStr);
        Vector3 pos = JsonUtility.FromJson<Vector3>(posStr);
        transform.position = pos;
        Physics.SyncTransforms();
    }
}


