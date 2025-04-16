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
    public CameraController cameraController;
    public PlayerIKHandler ikHandler;
    public Gun equipment;
    
    
    private CharacterController controller;
    private Vector3 velocity;
    
    
    private Vector2 moveInput;
    public bool isAiming;
    private bool isJumping;
    private bool isRunning;
    private bool isFiring;
    private bool isAimFiring;
    private bool isCrouching;
    private bool isCrouchAiming;
    private bool isMoving;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInput();
        ApplyGravity();
        
        cameraController.SetAiming(isAiming);
        cameraController.SetCrouch(isCrouchAiming);
        ikHandler.AimIK(isAiming);
        equipment.SetFire(isAimFiring);
        cameraController.SetShake(isAimFiring);
        
        if (IsGrounded() && isJumping)
        {
            Jump();
        }
        if (isAiming)
        {
            
            Move(aimwalkSpeed);
        }

        else if (isRunning && !isAiming &&!isCrouching)
        {
            
            Move(runSpeed);
      
            
        }
        else if (moveInput.magnitude > 0.1f)
        {
            if (isMoving&&!isCrouching)
            {
              
                Move(walkSpeed);
            }
            if (isCrouching)
            {
                
                Move(aimwalkSpeed);
            }
        }
        
        UpdateAnimation();
    }


    
    private void LateUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f || isAiming)
        {
            RotateToDirection();
        }
    }

    private void HandleInput()
    {

        
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isMoving = moveInput.magnitude > 0.1f;
        isAiming = Input.GetMouseButton(1);
        isFiring = Input.GetMouseButton(0);
        isJumping = Input.GetKeyDown(KeyCode.Space);
        isRunning = Input.GetKey(KeyCode.LeftShift);
        

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching; 
        }

        if (isAiming && isFiring)
        {
            isAimFiring = true;
        }
        else
        {
            isAimFiring = false;
        }
        if (isAiming && isCrouching)
        {
            isCrouchAiming = true;
        }
        else
        {
            isCrouchAiming = false;
        }
    }


    private void UpdateAnimation()
    {
        if (moveInput.magnitude > 0.1f)
        {
            animator.SetBool("Move", true);

            if (isJumping)
            {
                animator.SetTrigger("RunJump");
            }
        }
        if (moveInput.magnitude < 0.1f)
        {
            animator.SetBool("Move", false);
            
            if (isJumping)
            {
                animator.SetTrigger("Jump");
            }
        }

        if (isRunning && !isAiming)
        {
            animator.SetBool("Run", true);
        }
        if (!isRunning || isAiming)
        {
            animator.SetBool("Run", false);
        }

        animator.SetBool("Crouch", isCrouching);
        
       
    }
    private void RotateToDirection()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 targetDirection = Vector3.zero;

        if (isAiming)
        {
            // 조준 중: 카메라 정면 기준으로 약간 오른쪽 회전
            Quaternion rot = Quaternion.AngleAxis(50f, Vector3.up); // 카메라 기준 오프셋
            targetDirection = rot * camForward;
        }
        else if (moveInput.sqrMagnitude > 0.01f)
        {
            // 조준 아닐 때는 move 방향을 정확히 보도록 회전
            targetDirection = camForward * moveInput.y + camRight * moveInput.x;
        }

        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        
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
            // 상승 중과 하강 중의 중력 차등 적용
            float fallMultiplier = 2.5f; // 하강 시 더 빠르게 떨어짐
            if (velocity.y < 0)
            {
                // 낙하 중일 때 중력을 더 강하게 적용
                velocity.y += gravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }
        }

        controller.Move(Vector3.up * (velocity.y * Time.deltaTime));
    }

    private bool IsGrounded()
    {
        return controller.isGrounded;
    }
}
