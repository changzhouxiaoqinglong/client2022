﻿using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// Demo script provided by Unity Community Wiki - wiki.unity3d.com
/// </summary>

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class RigidbodyFPSController : MonoBehaviour
{

    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;
    public float minimumY = -60f;
    public float maximumY = 60f;

    private bool grounded = false;
    private float rotationY = 0f;
    private Rigidbody rb;

    [SerializeField]
    private Animator m_Animator = null;

    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    private void Update()
    {
       
        //if (Input.GetKey (KeyCode.W))
        if(Keyboard.current.wKey.wasPressedThisFrame)
        {
            m_Animator.SetBool("paobu", true);
            m_Animator.SetBool("dunxia", false);
            m_Animator.SetBool("daiji", false);
        }
        
        //if (Input.GetKeyUp(KeyCode.W))
        if(Keyboard.current.wKey.wasReleasedThisFrame)
        {
            m_Animator.SetBool("paobu", false);

        }
       
    }
    void FixedUpdate()
    {
        if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = Vector3.zero; //??//new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump
            //if (canJump && Input.GetKeyDown(KeyCode.Space))
            if (canJump && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }
        }

        // Rotation
        //float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
        //rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

        float rotationX = transform.localEulerAngles.y + Mouse.current.delta.ReadValue().x * sensitivityX;
        rotationY += Mouse.current.delta.ReadValue().y * sensitivityY;

        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

        // We apply gravity manually for more tuning control
        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));

        grounded = false;
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}