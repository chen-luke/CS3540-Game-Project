using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 2f;
    public float jumpAmt = 5f;
    private float superSpeed;
    Rigidbody rb;
    AudioSource jumpSFX;
    void Start()
    {
        // get a reference to the rigidbody component
        rb = GetComponent<Rigidbody>();
        jumpSFX = GetComponent<AudioSource>();
        // rb.AddForce(transform.forward * 5);
    }

    // Update is called once per frame
    // For physics calls use FixedUpdate. FixedUpdate will get called before calling Update()
    void FixedUpdate()
    {
        if(!LevelManager.isGameOver) {

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 forceVector = new Vector3(moveHorizontal, 0.0f, moveVertical);

            rb.AddForce(forceVector * speed);
        }
        else {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (transform.position.y < 1) {

                rb.AddForce(0, jumpAmt, 0, ForceMode.Impulse);
                jumpSFX.Play();

            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {

            // Double the player speed when pressing left shift.
            superSpeed = speed * 2;

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 forceVector = new Vector3(moveHorizontal, 0.0f, moveVertical);
            
            rb.AddForce(forceVector * superSpeed, ForceMode.VelocityChange);

        }

    }

    void ControlPlayer()
    {
        // get and store horizontal and vertical input

        // declare a forceVector based on h and v inputs

        // apply force to the rigidbody using forceVector
    }
}
