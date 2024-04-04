using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PlayerFSMController : MonoBehaviour
{
    public enum FSMStates
    {
        Idle, Moving, Jump, Attacking, Die
    }
    public FSMStates currentState;
    public float speed = 3.0f;
    public float sprintSpeedScalar = 1.5f;
    public float jumpForceScalar = 1.0f;
    public float superJumpForceScalar = 4f;
    public float gravity = 9.81f;
    public float airControl = 1.25f;
    public float coyoteTime = 0.3f;
    public int heavyManaCost = 10;
    public AudioClip lightAttackSFX;
    public AudioClip heavyAttackSFX;
    CharacterController cc;
    float moveAngleFromRight;
    Vector3 input, moveDirection;
    Animator anim;
    PlayerHealth playerHealth;
    PlayerMana playerMana;
    ShootProjectile shootProjectile;
    private float minHeight = 26f;
    private float moveSpeed;
    private float jumpAmount;
    private bool wasSprinting = false;
    private bool canJump = true;
    private bool attackAnimLock = false;
    private bool isLightAttack;
    // This is a temporary fix for the attack animation freezing the player at the end for a few frames
    public float attackAnimFreezeOffset = 0.48f;
    private float lightAttackDuration;
    private float heavyAttackDuration;
    private float canJumpTimer;

    void Start()
    {
        Initialize();
        currentState = FSMStates.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelManager.isGameOver && Time.timeScale != 0)
        {
            if (transform.position.y < minHeight)
            {
                playerHealth.PlayerDies();
                currentState = FSMStates.Die;
            }
            if (PlayerHealth.isDead)
            {
                currentState = FSMStates.Die;
            }

            if (cc.isGrounded)
            {
                canJumpTimer = coyoteTime;
            }
            else if (canJumpTimer > 0)
            {
                canJumpTimer -= Time.deltaTime;
            }

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;
            moveAngleFromRight = Vector3.Angle(transform.right, input);
            jumpAmount = Input.GetKey(KeyCode.LeftShift) && LevelManager.bootsPickedUp ? superJumpForceScalar : jumpForceScalar;


            switch (currentState)
            {
                case FSMStates.Idle:
                    UpdateIdleState();
                    break;
                case FSMStates.Moving:
                    UpdateMovingState();
                    break;
                case FSMStates.Jump:
                    UpdateJumpState();
                    break;
                case FSMStates.Attacking:
                    UpdateAttackingState();
                    break;
                case FSMStates.Die:
                    UpdateDieState();
                    break;
            }
        }
        else if (PlayerHealth.isDead)
        {
            currentState = FSMStates.Die;
            UpdateDieState();
        }

        if (input.magnitude > 0.01f) {
            float cameraYawRotation = Camera.main.transform.eulerAngles.y;
            Quaternion newRotation = Quaternion.Euler(0f, cameraYawRotation, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 10);
        }

    }

    void Initialize()
    {
        cc = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        playerMana = GetComponent<PlayerMana>();
        shootProjectile = gameObject.GetComponent<ShootProjectile>();
        if (File.Exists(LevelManager.savePointJSONPath))
        {
            SetPosition();
        }
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "LightAttack")
            {
                lightAttackDuration = clip.length;
            }
            else if (clip.name == "HeavyAttack")
            {
                heavyAttackDuration = clip.length;
            }
        }
    }

    void UpdateIdleState()
    {
        anim.SetInteger("animState", 0);
        if (Input.GetButtonDown("Fire1") && Time.timeScale != 0)
        {
            isLightAttack = true;
            currentState = FSMStates.Attacking;
        }
        else if (Input.GetKey(KeyCode.F) && LevelManager.glovePickedUp && playerMana.GetCurrentMana() >= heavyManaCost)
        {
            isLightAttack = false;
            currentState = FSMStates.Attacking;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && canJumpTimer > 0)
        {
            currentState = FSMStates.Jump;
        }
        else if (input.magnitude > 0)
        {
            currentState = FSMStates.Moving;
        }
        else
        {
            wasSprinting = Input.GetKeyDown(KeyCode.LeftShift);
            cc.Move(new Vector3(0, -gravity, 0) * Time.deltaTime);
        }
    }

    void UpdateMovingState()
    {
        // print(cc.isGrounded);
        if (Input.GetKeyDown(KeyCode.Space) && canJumpTimer > 0) //cc.isGrounded being unreliable
        {
            currentState = FSMStates.Jump;
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            isLightAttack = true;
            currentState = FSMStates.Attacking;
        }
        else if (Input.GetKey(KeyCode.F) && LevelManager.glovePickedUp && playerMana.GetCurrentMana() >= heavyManaCost)
        {
            isLightAttack = false;
            currentState = FSMStates.Attacking;
        }
        else if (input.magnitude == 0)
        {
            currentState = FSMStates.Idle;
        }
        else
        {
            if (moveAngleFromRight < 45)
            {
                anim.SetInteger("animState", 3);
            }
            else if (moveAngleFromRight >= 45 && moveAngleFromRight <= 135)
            {
                int animStateNum = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
                anim.SetInteger("animState", animStateNum);
            }
            else if (moveAngleFromRight > 135)
            {
                anim.SetInteger("animState", 4);
            }
            MovePlayer(true);
        }
    }

    void UpdateJumpState()
    {
        anim.SetInteger("animState", 5);
        if (!canJump && cc.isGrounded)
        {
            currentState = input.magnitude > 0 ? FSMStates.Moving : FSMStates.Idle;
            canJump = true;
        }
        else
        {
            if (canJump)
            {
                moveSpeed = wasSprinting ? speed * sprintSpeedScalar : speed;
                moveDirection = input * moveSpeed;
                moveDirection.y = Mathf.Sqrt(2 * jumpAmount * gravity);
                canJump = false;
            }
            else
            {
                moveSpeed = wasSprinting ? speed * sprintSpeedScalar : speed;
                moveDirection = Vector3.Lerp(moveDirection, input * moveSpeed * airControl, Time.deltaTime);
                if (Input.GetButton("Fire1"))
                {
                    isLightAttack = true;
                    currentState = FSMStates.Attacking;
                }
            }
            moveDirection.y -= gravity * Time.deltaTime;
            cc.Move(moveDirection * Time.deltaTime);
        }
    }

    void UpdateAttackingState()
    {
        if (isLightAttack && !attackAnimLock)
        {
            // Light Attack

            anim.SetInteger("animState", 6);
            attackAnimLock = true;
            AudioSource.PlayClipAtPoint(lightAttackSFX, Camera.main.transform.position, volume: 0.8f); // Need to delay for animation
            Invoke("UnlockAttackAnim", lightAttackDuration - attackAnimFreezeOffset);
        }
        else if (!isLightAttack && !attackAnimLock)
        {
            // Heavy Attack
            anim.SetInteger("animState", 7);
            attackAnimLock = true;
            // AudioSource.PlayClipAtPoint(heavyAttackSFX, Camera.main.transform.position); // Need to delay for animation
            Invoke("HeavyAttack", heavyAttackDuration - attackAnimFreezeOffset - .6f);
            Invoke("UnlockAttackAnim", heavyAttackDuration - attackAnimFreezeOffset);
            playerMana.UseMana(heavyManaCost);
        }
        else
        {
            if (anim.GetInteger("animState") == 6)
            {
                MovePlayer(false);
            }
            else
            {
                cc.Move(new Vector3(0, -gravity, 0) * Time.deltaTime);
            }
        }
    }

    void UpdateDieState()
    {
        anim.SetInteger("animState", 8);
    }

    void UnlockAttackAnim()
    {
        attackAnimLock = false;
        currentState = input.magnitude > 0 ? FSMStates.Moving : FSMStates.Idle;
    }

    void MovePlayer(bool withSprint)
    {
        moveSpeed = speed;
        if (cc.isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift) && withSprint)
            {
                moveSpeed *= sprintSpeedScalar;
                wasSprinting = true;
            }
            else if (withSprint)
            {
                wasSprinting = false;
            }
            moveDirection = input * moveSpeed;
        }
        else
        {
            moveSpeed *= airControl;
            if (wasSprinting && withSprint)
            {
                moveSpeed *= sprintSpeedScalar;
            }
            moveDirection = Vector3.Lerp(moveDirection, input * moveSpeed, Time.deltaTime);
        }
        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);
    }

    public void SetPosition()
    {
        string posStr = File.ReadAllText(LevelManager.savePointJSONPath);
        // print(posStr);
        Vector3 pos = JsonUtility.FromJson<Vector3>(posStr);
        transform.position = pos;
        Physics.SyncTransforms();
    }

    private void HeavyAttack()
    {
        shootProjectile.ShootSlashProjectile();
        AudioSource.PlayClipAtPoint(heavyAttackSFX, Camera.main.transform.position, volume: 0.8f);
    }
}
