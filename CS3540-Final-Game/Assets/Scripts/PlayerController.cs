
using UnityEngine;

public class PlayerController : MonoBehaviour
{
// Start is called before the first frame update
    public float walkSpeed = 5.0f;
    public float sprintSpeedScalar = 2.0f;
    public float jumpForceScalar = 2.0f;
    public float gravity = 9.81f;
    public float airControl = 0.75f;
    CharacterController cc;
    private float moveSpeed;
    Vector3 input, moveDirection;

    private float minHeight = 26f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerHealth.isDead) {
            
            MovePlayer();
        }
        if (transform.position.y < minHeight) {
            PlayerHealth.isDead = true;
            // Would restart level in the future
            Destroy(gameObject);
        }
    }


    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        moveSpeed = Input.GetKey(KeyCode.LeftShift) && cc.isGrounded ? walkSpeed * sprintSpeedScalar : walkSpeed;

        input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;
        input *= moveSpeed;

        if (cc.isGrounded)
        {
            moveDirection = input;
            moveDirection.y = Input.GetButton("Jump") ? Mathf.Sqrt(2 * jumpForceScalar * gravity) : 0;
        }
        else
        {
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);
    }
}
