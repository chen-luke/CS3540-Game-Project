
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ThirdPersonController : MonoBehaviour
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
    Vector3 input, moveDirection;

    Animator m_Animator;

    private float minHeight = 26f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        m_Animator = gameObject.GetComponent<Animator>();
        if (LevelManager.savePoint)
        {
            SetPosition(LevelManager.savePoint);
        }
        PlayerHealth.isDead = false;

    }

    void Update() {
        if (input.magnitude > 0.01f) {
            float cameraYawRotation = Camera.main.transform.eulerAngles.y;
            Quaternion newRotation = Quaternion.Euler(0f, cameraYawRotation, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 10);
        }
    }
    void FixedUpdate()
    {
        if (!PlayerHealth.isDead)
        {

            MovePlayer();
            if (transform.position.y < minHeight)
            {
                gameObject.GetComponent<PlayerHealth>().PlayerDies();
                m_Animator.enabled = false;
                // PlayerHealth.isDead = true;
                // Would restart level in the future
                // Destroy(gameObject);
            }
        }
        if (transform.position.y < minHeight && !PlayerHealth.isDead)
        {
            PlayerHealth.isDead = true;
            FindObjectOfType<LevelManager>().LevelLost();
        }
    }


    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        moveSpeed = Input.GetKey(KeyCode.LeftShift) && cc.isGrounded ? walkSpeed * sprintSpeedScalar : walkSpeed;

        float jumpAmount = Input.GetKey(KeyCode.LeftShift) && cc.isGrounded && LevelManager.bootsPickedUp ? superJumpForceScalar : jumpForceScalar;

        
        input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;
        input *= moveSpeed;

        if (cc.isGrounded)
        {
            moveDirection = input;


            if (input.x > 0 || input.x < 0 || input.z > 0 || input.z < 0)
            {
                if (Input.GetKey("a"))
                {
                    StrafeLeftAnimation();
                }
                else if (Input.GetKey("d"))
                {
                    StrafeRightAnimation();
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    SprintAnimation();
                }
                else
                {
                    RunAnimation();
                }
            }
            else if(m_Animator.GetInteger("animState") != 3)
            {
                IdleAnimation();
            }

            if (Input.GetButton("Jump"))
            {

                moveDirection.y = Mathf.Sqrt(2 * jumpAmount * gravity);
                JumpAnimation();
            }
            else
            {
                moveDirection.y = 0.0f;

            }
        }
        else
        {
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);
    }

    private void IdleAnimation()
    {
        m_Animator.SetInteger("animState", 0);
    }

    private void RunAnimation()
    {
        m_Animator.SetInteger("animState", 2);
    }

    private void JumpAnimation()
    {
        m_Animator.SetInteger("animState", 4);
    }

    private void StrafeLeftAnimation()
    {
        m_Animator.SetInteger("animState", 1);
    }
    private void StrafeRightAnimation()
    {
        m_Animator.SetInteger("animState", 5);
    }

    private void SprintAnimation()
    {
        m_Animator.SetInteger("animState", 6);
    }

    private void DeathAnimation()
    {
        m_Animator.SetInteger("animState", 7);
    }

    public void SetPosition(Transform newTransform)
    {
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
        Physics.SyncTransforms();
    }
}

