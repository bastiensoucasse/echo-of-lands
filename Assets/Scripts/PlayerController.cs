using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(1, 10)] public float m_LookSensivity = 6F;
    [Range(1, 10)] public float m_MoveSpeed = 6F;
    public float m_LookSensivityMultiplier = 100F;
    public float m_MinSprintMultiplier = 0.5F;
    public float m_InitialSprintMultiplier = 2F;
    public float m_MaxSprintMultiplier = 2F;
    public float m_JumpHeight = 1F;
    public float m_FieldOfViewSpeed = 10F;
    public LayerMask m_GroundMask;
    public float m_GroundCheckDistance = 0.1F;

    Camera m_Camera;
    CharacterController m_CharacterController;
    float m_XRotation, m_YRotation;
    Vector3 m_Move, m_Velocity;
    bool m_IsGrounded, m_IsSprinting, m_IsCrouched;
    float m_FieldOfView, m_PlayerHeight, m_SprintMultiplier;

    void Start()
    {
        m_Camera = Camera.main;
        m_CharacterController = GetComponent<CharacterController>();

        m_FieldOfView = m_Camera.fieldOfView;
        m_PlayerHeight = m_CharacterController.height;
        m_SprintMultiplier = m_InitialSprintMultiplier;

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
        m_XRotation -= Input.GetAxis("Camera Y") * m_LookSensivityMultiplier * m_LookSensivity * Time.deltaTime;
        m_XRotation = Mathf.Clamp(m_XRotation, -80F, 80F);

        m_YRotation += Input.GetAxis("Camera X") * m_LookSensivityMultiplier * m_LookSensivity * Time.deltaTime;

        UpdateCrouch();
        UpdateSprint();

        UpdateMoveMultiplier();

        m_Move = (transform.right * Input.GetAxis("Move X") + transform.forward * Input.GetAxis("Move Y")) * m_SprintMultiplier;

        if (m_Velocity.y < 0F && m_IsGrounded) m_Velocity.y = 0F;

        UpdateJump();

        m_Velocity.y += Physics.gravity.y * Time.deltaTime;
    }

    void UpdateCrouch()
    {
        if (Input.GetButtonDown("Crouch") && m_IsGrounded)
        {
            if (m_IsCrouched) m_IsCrouched = false;
            else if (!m_IsSprinting) m_IsCrouched = true;
        }
    }

    void UpdateSprint()
    {
        if (Input.GetButtonDown("Sprint") || Input.GetAxis("Sprint") > 0.5F && Input.GetAxis("Move X") > -0.4F && Input.GetAxis("Move X") < 0.4F && Input.GetAxis("Move Y") > 0.6F)
        {
            if (m_IsCrouched) m_IsCrouched = false;
            m_IsSprinting = true;
        }
        else m_IsSprinting = false;
    }

    void UpdateMoveMultiplier()
    {

        if (m_IsCrouched)
        {
            if (m_SprintMultiplier > m_MinSprintMultiplier) m_SprintMultiplier -= 0.05F;
            if (m_SprintMultiplier < m_MinSprintMultiplier) m_SprintMultiplier = m_MinSprintMultiplier;
        }
        else if (m_IsSprinting)
        {
            if (m_SprintMultiplier < m_MaxSprintMultiplier) m_SprintMultiplier += 0.01F;
            if (m_SprintMultiplier > m_MaxSprintMultiplier) m_SprintMultiplier = m_MaxSprintMultiplier;
        }
        else
        {
            if (m_SprintMultiplier < m_InitialSprintMultiplier) m_SprintMultiplier += 0.05F;
            if (m_SprintMultiplier > m_InitialSprintMultiplier) m_SprintMultiplier -= 0.02F;

            if ((m_SprintMultiplier > m_InitialSprintMultiplier - 0.05F && m_SprintMultiplier < m_InitialSprintMultiplier) ||
                    (m_SprintMultiplier < m_InitialSprintMultiplier + 0.05F && m_SprintMultiplier > m_InitialSprintMultiplier))
                m_SprintMultiplier = m_InitialSprintMultiplier;
        }
    }

    void UpdateJump()
    {
        if (Input.GetButtonDown("Jump") && m_IsGrounded)
        {
            if (m_IsCrouched) m_IsCrouched = false;
            m_Velocity.y = Mathf.Sqrt(m_JumpHeight * -2F * Physics.gravity.y);
        }
    }

    void RotatePlayer()
    {
        m_Camera.transform.localRotation = Quaternion.Euler(Vector3.right * m_XRotation);
        transform.localRotation = Quaternion.Euler(Vector3.up * m_YRotation);
    }

    void MovePlayer()
    {
        m_CharacterController.Move(m_Move * m_MoveSpeed * Time.deltaTime);
        m_CharacterController.Move(m_Velocity * Time.deltaTime);

        if (m_IsSprinting)
            m_Camera.fieldOfView = Mathf.MoveTowards(m_Camera.fieldOfView, m_FieldOfView + 10F, m_FieldOfViewSpeed * Time.deltaTime);
        else
            m_Camera.fieldOfView = Mathf.MoveTowards(m_Camera.fieldOfView, m_FieldOfView, m_FieldOfViewSpeed * Time.deltaTime);

        if (m_IsCrouched) m_CharacterController.height = m_PlayerHeight - 0.8F;
        else m_CharacterController.height = m_PlayerHeight;
        m_Camera.transform.position = Vector3.up * (m_CharacterController.height - 0.1F) + transform.position;
    }
}
