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
    

    public PlayerWeapon playerWeapon;
    
    private CharacterController controller;
    private Vector3 velocity;
    
    
    
    
    private Vector2 moveInput;
    public bool isAiming;
    private bool isJumping;
    private bool isRunning;
    private bool isFiring;
    public bool IsCrouching {get; private set;}

    public bool isReloading;
    private bool reloadStarted = false;
    
    private bool isCrouchAiming;
    private bool isMoving;
    private bool isDead;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isDead) return;
      
        HandleInput();
        ApplyGravity();
        
        cameraController.SetAiming(isAiming);
        cameraController.SetCrouch(isCrouchAiming);
        ikHandler.AimIK(isAiming);
        
        if (IsGrounded() && isJumping)
        {
            Jump();
        }
        
        if (isAiming)
        {
            
            Move(aimwalkSpeed);

            if (isFiring)
            {
                playerWeapon.Fire();
            }
        }
        
        if (isRunning && !isAiming &&!IsCrouching)
        {
            
            Move(runSpeed);
            
        }
        else if (isMoving && !isAiming &&!IsCrouching)
        {

            Move(walkSpeed);
            
        }
        else if (IsCrouching&&!isAiming)
        {
                
            Move(aimwalkSpeed);
        }

        if (isReloading && !reloadStarted)
        {
            StartReload();
        }

        if (reloadStarted)
        {
            CheckReloadEnd();
        }
        
        UpdateAnimation();
    }
    
    
    private void LateUpdate()
    {
        if (isDead) return;
        RotateToDirection();
    }
    
    private void StartReload()
    {
        playerWeapon.Reload();   
        if(playerWeapon.noBullet) return;
        
        
        animator.ResetTrigger("Reload"); // 이전 트리거 제거
        animator.SetTrigger("Reload");
        animator.SetLayerWeight(3, 1f);
    
        ikHandler.SetReloading(true);
        reloadStarted = true;
    }
    private void CheckReloadEnd()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(3); // UpperLayer
        if (stateInfo.IsName("Reload") && stateInfo.normalizedTime >= 0.9f)
        {
            Debug.Log("장전 애니메이션 종료");

            animator.SetLayerWeight(3, 0f);
            animator.ResetTrigger("Reload"); 

            ikHandler.SetReloading(false);

            reloadStarted = false;
            isReloading = false;
        }
    }
    private void HandleInput()
    {

        
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isMoving = moveInput.magnitude > 0.1f;
    
        isAiming = Input.GetMouseButton(1)&&!reloadStarted; 
        
        isFiring = Input.GetMouseButton(0);
        isJumping = Input.GetKeyDown(KeyCode.Space);
        isRunning = Input.GetKey(KeyCode.LeftShift);
      

        if (Input.GetKeyDown(KeyCode.C))
        {
            IsCrouching = !IsCrouching; 
        }
        if (Input.GetKeyDown(KeyCode.R) && !reloadStarted) 
        {
            isReloading = true;
        }
        
        if (isAiming && IsCrouching)
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

        animator.SetBool("Crouch", IsCrouching);
        
       
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
            
            Quaternion rot = Quaternion.AngleAxis(30f, Vector3.up); 
            targetDirection = rot * camForward;
        }
        else 
        {
            
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

   
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        moveDir.Normalize();
        
     

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
        
            float fallMultiplier = 2.5f; 
            if (velocity.y < 0)
            {
          
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

    public void OnPlayerDeath()
    {
        isDead = true;

        animator.applyRootMotion = true; 
        animator.SetTrigger("Die");
   
        ikHandler.AimIK(false);

        isReloading = false;
        reloadStarted = false;
        isAiming = false;
    }


}
