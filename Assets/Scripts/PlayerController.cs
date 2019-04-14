﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the input and movement for the player character.
/// This script should stay shorter than 200 lines.
/// </summary>
public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;

    Rigidbody rb;
    AbilityManager abilityManager;

    private bool m_isGrounded = false;

    public float m_normalMoveSpeed = 30f;
    public float m_moveSpeed = 30f;
    public float m_runMultiplyFactor = 3f;
    public float m_turnSpeed = 2f;
    public float m_maxSpeed = 5f;
    public float m_jumpPower = 0f;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        abilityManager = GetComponent<AbilityManager>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput);

        //determine movement in respect to camera
        Vector3 cameraForward = Camera.main.transform.TransformDirection(Vector3.forward) * 10;
        Vector3 cameraForwardNoY = new Vector3(cameraForward.x, 0, cameraForward.z);

        Vector3 cameraRight = Camera.main.transform.TransformDirection(Vector3.right) * 10;
        Vector3 cameraRightNoY = new Vector3(cameraRight.x, 0, cameraRight.z);

        //running
        if (abilityManager.canRun)
        {
            if (Input.GetAxis("Fire3") > 0.1f)
            {
                m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_normalMoveSpeed * m_runMultiplyFactor, 2f * Time.deltaTime);
            }
            else
            {
                m_moveSpeed = m_normalMoveSpeed;
            }
        }

        //movement
        rb.AddForce(m_moveSpeed * (verticalInput * cameraForwardNoY));
        transform.Rotate(horizontalInput * Vector3.up);

        //limit the velocity
        if (rb.velocity.magnitude > m_maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.normalized.x * m_maxSpeed, rb.velocity.y, rb.velocity.normalized.z * m_maxSpeed);
        }

        Debug.DrawRay(transform.position, cameraForward, Color.green);

        //friction
        float frictionAdjust = 0.93f;
        float maxVelocityY = 5f;
        rb.velocity = new Vector3(frictionAdjust * rb.velocity.x, Mathf.Min(maxVelocityY, rb.velocity.y), frictionAdjust * rb.velocity.z);

        //look in the direction of horizontal input over time
        //if (Mathf.Abs(horizontalInput) > 0.2f)
        //{
        //    RotateTowardsTarget(transform.position + transform.TransformDirection(horizontalInput * Vector3.right), Mathf.Abs(horizontalInput) * m_turnSpeed);
        //}

        //jumping
        if (abilityManager.canJump)
        {
            if (Input.GetAxis("Jump") > 0.1f)
            {
                if (m_isGrounded)
                {
                    Jump();
                }
            }
        }

        //check whether to ground
        if (!m_isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
            {
                m_isGrounded = true;
            }
        }
    }

    private void RotateTowardsTarget(Vector3 target, float speed)
    {
        Vector3 targetPos = target;
        Vector3 relativePos = targetPos - transform.position;

        Quaternion desiredRot = Quaternion.LookRotation(relativePos, Vector3.up);
        Quaternion newRot = Quaternion.Lerp(transform.rotation, desiredRot, speed * Time.deltaTime);
        transform.rotation = newRot;
    }

    private void Jump()
    {
        rb.AddForce(m_jumpPower * Vector3.up);
        m_isGrounded = false;
    }
}
