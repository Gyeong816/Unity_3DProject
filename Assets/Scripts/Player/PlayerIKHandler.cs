using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKHandler : MonoBehaviour
{
    private Animator animator;

    public Gun currentEquipment;
    public Transform gunAimTarget;
    public Transform bodyAimTarget;
    
    public Transform handPosition;
    
    public Transform gunPosition;
    
    [Range(0f, 1f)] public float bodyWeight = 0.5f;
    [Range(0f, 1f)] public float headWeight = 0.5f;
    [Range(0f, 1f)] public float clampWeight = 0.5f;

    public float weaponRotateSpeed = 10f;
    public bool lockYRotation = false;

    private bool playerAiming = false;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AimIK(bool aim)
    {
        playerAiming = aim;
    }

 private void OnAnimatorIK(int layerIndex)
 {
    if (animator == null || currentEquipment == null) return;

    float rightIKWeight = playerAiming ? currentEquipment.IKWeight : 0f;
    float leftIKWeight = currentEquipment.IKWeight;

    Transform leftHandTarget = currentEquipment.LeftHandTarget;
    Transform rightHandTarget = currentEquipment.RightHandTarget;
    Transform weapon = currentEquipment.transform;
    Transform anchor = gunPosition;

    
    UpdateIK(AvatarIKGoal.LeftHand, leftHandTarget, leftIKWeight);
    UpdateIK(AvatarIKGoal.RightHand, rightHandTarget, rightIKWeight);
    

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
