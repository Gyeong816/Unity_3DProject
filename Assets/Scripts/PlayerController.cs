using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   [Header("Movement")]
    public float walkSpeed = 4f;
    public float aimwalkSpeed = 2f;
    public float runSpeed = 7f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;
    public CameraController cameraFollow;
    
    
    private CharacterController controller;
    private Vector3 velocity;

    public GunController gun;
    
    
    private Vector2 moveInput;
    private bool isAiming;
    private bool isJumping;
    private bool isRunning;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInput();
        
        if (Input.GetButtonDown("Fire1")) // 왼쪽 마우스 클릭
        {
            gun.Fire();  // 여기서 호출
        }
        
        cameraFollow.SetAiming(isAiming);
        
        ApplyGravity();
        
        if (IsGrounded() && isJumping)
        {
            Jump();
        }

        // 상태별 이동 및 애니메이션 처리
        if (isAiming)
        {
            
            Move(aimwalkSpeed);
           
        }
        else if (isRunning && !isAiming)
        {
            
            Move(runSpeed);
      
            
        }
        else if (moveInput.magnitude > 0.1f)
        {
            
            Move(walkSpeed);
   
        }

        UpdateAnimation();
    }

    private void ChangeCamera()
    {
        
    }
    
    private void LateUpdate()
    {
        // 움직이거나 조준 중일 때만 회전
        if (moveInput.sqrMagnitude > 0.01f || isAiming)
        {
            RotateToDirection();
        }
    }

    private void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isAiming = Input.GetMouseButton(1);
        isJumping = Input.GetKeyDown(KeyCode.Space);
        isRunning = Input.GetKey(KeyCode.LeftShift);
    }


    private void UpdateAnimation()
    {
        if (moveInput.magnitude > 0.1f)
        {
            animator.SetBool("Move", true);
        }
        if (moveInput.magnitude < 0.1f)
        {
            animator.SetBool("Move", false);
        }

        if (isRunning && !isAiming)
        {
            animator.SetBool("Run", isRunning);
        }
        if (!isRunning)
        {
            animator.SetBool("Run", false);
        }
        
    }
    private void RotateToDirection()
    {
        Vector3 targetDirection;

        if (isAiming)
        {
            // 조준 시: 카메라 방향 기준 회전
            targetDirection = cameraTransform.forward;
        }
        else
        {
            // 이동 방향 기준 회전
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            targetDirection = camForward * moveInput.y + camRight * moveInput.x;
        }

        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

    
            // 가만히 → 부드럽게 회전
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        
    }


    private void Move(float speed, bool aiming = false)
    {
        if (moveInput.sqrMagnitude <= 0.01f) return;

        // 카메라 기준 방향 계산
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        moveDir.Normalize();

        // 조준 중일 경우 애니메이션 파라미터 설정
        if (aiming && animator != null)
        {
            animator.SetFloat("InputX", moveInput.x, 0.1f, Time.deltaTime);
            animator.SetFloat("InputY", moveInput.y, 0.1f, Time.deltaTime);
        }

        controller.Move(moveDir * (speed * Time.deltaTime));
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(Vector3.up * (velocity.y * Time.deltaTime));
    }

    private bool IsGrounded()
    {
        return controller.isGrounded;
    }
}
