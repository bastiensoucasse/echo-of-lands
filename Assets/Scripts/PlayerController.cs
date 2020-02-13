using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(1, 10)] public float m_LookSensivity = 6F;
    [Range(1, 10)] public float m_MoveSpeed = 6F;
    public float m_LookSensivityMultiplier = 100F;
    public float m_MoveSpeedMultiplier = 2F;
    public float m_JumpHeight = 1F;
    public LayerMask m_GroundMask;
    public float m_GroundCheckDistance = 0.1F;

    Camera m_Camera;
    CharacterController m_CharacterController;
    float m_XRotation, m_YRotation;
    Vector3 m_Move, m_Velocity;
    bool m_IsGrounded;

    void Start()
    {
        m_Camera = Camera.main;
        m_CharacterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckGroundStatus();
        UpdateMove();
    }

    void FixedUpdate()
    {
        RotatePlayer();
        MovePlayer();
    }

    void CheckGroundStatus()
    {
        m_IsGrounded = Physics.CheckSphere(transform.position, m_GroundCheckDistance, m_GroundMask);
    }

    void UpdateMove()
    {
        m_XRotation -= Input.GetAxis("Input Y") * m_LookSensivityMultiplier * m_LookSensivity * Time.deltaTime;
        m_XRotation = Mathf.Clamp(m_XRotation, -80F, 80F);

        m_YRotation += Input.GetAxis("Input X") * m_LookSensivityMultiplier * m_LookSensivity * Time.deltaTime;

        m_Move = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));

        if (m_Velocity.y < 0F && m_IsGrounded)
            m_Velocity.y = 0F;

        if (Input.GetButtonDown("Jump") && m_IsGrounded)
            m_Velocity.y = Mathf.Sqrt(m_JumpHeight * -2F * Physics.gravity.y);

        m_Velocity.y += Physics.gravity.y * Time.deltaTime;
    }

    void RotatePlayer()
    {
        m_Camera.transform.localRotation = Quaternion.Euler(Vector3.right * m_XRotation);
        transform.localRotation = Quaternion.Euler(Vector3.up * m_YRotation);
    }

    void MovePlayer()
    {
        m_CharacterController.Move(m_Move * m_MoveSpeedMultiplier * m_MoveSpeed * Time.deltaTime);
        m_CharacterController.Move(m_Velocity * Time.deltaTime);
    }
}
