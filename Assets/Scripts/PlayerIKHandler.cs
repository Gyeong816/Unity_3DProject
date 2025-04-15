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
    Transform weapon = currentEquipment.WeaponTransform;
    Transform anchor = gunPosition;

    UpdateIK(AvatarIKGoal.LeftHand, leftHandTarget, leftIKWeight);
    UpdateIK(AvatarIKGoal.RightHand, rightHandTarget, rightIKWeight);

    if (weapon == null || aimTarget == null || anchor == null || handPosition == null) return;

    if (playerAiming)
    {
        // 조준 중: 무기를 앵커에 붙이고, 앵커 기준으로 조준 방향 회전
        if (weapon.parent != anchor)
        {
            weapon.SetParent(anchor, false);
            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.identity;
        }

        animator.SetLookAtWeight(1f, bodyWeight, headWeight, 0f, clampWeight);
        animator.SetLookAtPosition(aimTarget.position);

        Vector3 direction = aimTarget.position - anchor.position;
       

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
            weapon.rotation = Quaternion.Slerp(weapon.rotation, targetRot, Time.deltaTime * weaponRotateSpeed);
        }
    }
    else
    {
        // 조준 해제: 무기를 손 위치에 붙이고 정렬
        if (weapon.parent != handPosition)
        {
            weapon.SetParent(handPosition, false);
            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.identity;
        }
    }

    // 내부 함수 - IK 갱신
    void UpdateIK(AvatarIKGoal goal, Transform target, float weight)
    {
        if (target == null || weight <= 0f)
        {
            animator.SetIKPositionWeight(goal, 0f);
            animator.SetIKRotationWeight(goal, 0f);
            return;
        }

        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKRotationWeight(goal, weight);
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation);
    }
  }
}
