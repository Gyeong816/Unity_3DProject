using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKHandler : MonoBehaviour
{
    private Animator animator;


    public PlayerWeapon playerWeapon;
   
    public Transform gunAimTarget;
    public Transform bodyAimTarget;
    
    public Transform handPosition;
    
    public Transform gunPosition;
    
    private bool isReloading = false;
    
    [Range(0f, 1f)] public float bodyWeight = 0.5f;
    [Range(0f, 1f)] public float headWeight = 0.5f;
    [Range(0f, 1f)] public float clampWeight = 0.5f;

    public float weaponRotateSpeed = 10f;
    public bool lockYRotation = false;

    private bool playerAiming = false;
    
    
    private float leftIKCurrentWeight = 1f;
    private float rightIKCurrentWeight = 1f;
    public float ikBlendSpeed = 2f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AimIK(bool aim)
    {
        playerAiming = aim;
    }
    public void SetReloading(bool reloading)
    {
        isReloading = reloading;
    }
    
    void UpdateIKWeight()
    {
        float targetLeft = isReloading ? 0f : playerWeapon.IKWeight;
        float targetRight = playerAiming ? playerWeapon.IKWeight : 0f;

        leftIKCurrentWeight = Mathf.Lerp(leftIKCurrentWeight, targetLeft, Time.deltaTime * ikBlendSpeed);
        rightIKCurrentWeight = Mathf.Lerp(rightIKCurrentWeight, targetRight, Time.deltaTime * ikBlendSpeed);
    }

 private void OnAnimatorIK(int layerIndex)
 {
    if (animator == null || playerWeapon == null) return;

   
    
  

    Transform leftHandTarget = playerWeapon.LeftHandTarget;
    Transform rightHandTarget = playerWeapon.RightHandTarget;
    Transform weapon = playerWeapon.GetCurrentWeaponTransform();
    Transform anchor = gunPosition;

    UpdateIKWeight(); // 보간 먼저
    UpdateIK(AvatarIKGoal.LeftHand, leftHandTarget, leftIKCurrentWeight);
    UpdateIK(AvatarIKGoal.RightHand, rightHandTarget, rightIKCurrentWeight);
    

    if (playerAiming)
    {
       
        if (weapon.parent != anchor)
        {
            weapon.SetParent(anchor, false);
            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.identity;
        }

        animator.SetLookAtWeight(1f, bodyWeight, headWeight, 0f, clampWeight);
        animator.SetLookAtPosition(bodyAimTarget.position);

        
        
        Vector3 direction = gunAimTarget.position - anchor.position;
        
        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
        weapon.rotation = Quaternion.Slerp(weapon.rotation, targetRot, Time.deltaTime * weaponRotateSpeed);
        
    }
    else
    {
        
        
        weapon.SetParent(handPosition, false);
        weapon.localPosition = Vector3.zero;
        weapon.localRotation = Quaternion.identity;
        
    }


    void UpdateIK(AvatarIKGoal goal, Transform target, float weight)
    {

        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKRotationWeight(goal, weight);
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation);
    }
  }
}
