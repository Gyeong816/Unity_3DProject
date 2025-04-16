using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKHandler : MonoBehaviour
{
    private Animator animator;

    public Equipment currentEquipment;
    public Transform aimTarget; // 카메라 중앙 기준 조준 지점
    
    public Transform handPosition;
    
    public Transform gunPosition;
    
    [Range(0f, 1f)] public float bodyWeight = 0.5f;
    [Range(0f, 1f)] public float headWeight = 0.5f;
    [Range(0f, 1f)] public float clampWeight = 0.5f;

    [Header("총기 회전 설정")] public float weaponRotateSpeed = 10f;
    public bool lockYRotation = false;

    private bool playerAiming = false;
    
    private float currentRightIKWeight = 0f;
    private float rightIKLerpSpeed = 5f;

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
        animator.SetLookAtPosition(aimTarget.position);

        Vector3 direction = aimTarget.position - anchor.position;
       
        Debug.DrawRay(anchor.position, direction.normalized * 100f, Color.blue);
        
           
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
