using System.Collections;
using UnityEngine;

public class EnemyIKHandler : MonoBehaviour
{
    private Animator animator;

    public EnemyAk47 enemyWeapon;

    // 여러 타겟을 받을 배열
    public Transform[] gunAimTargets;
    private Transform currentGunTarget; // 현재 조준 중인 타겟

    [Range(0f, 1f)] public float bodyWeight = 0.5f;
    [Range(0f, 1f)] public float headWeight = 0.5f;
    [Range(0f, 1f)] public float clampWeight = 0.5f;

    public float weaponRotateSpeed = 10f;
    public bool lockYRotation = false;

    private bool isEnemyAiming = false;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AimIK(bool aim)
    {
        isEnemyAiming = aim;
    }

    public void Die()
    {
        isDead = true;
    }

    
    public void PickRandomAimTarget()
    {
        if (gunAimTargets != null && gunAimTargets.Length > 0)
        {
            int index = Random.Range(0, gunAimTargets.Length);
            currentGunTarget = gunAimTargets[index];
        }
        else
        {
            currentGunTarget = null;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || enemyWeapon == null || isDead) return;

        float leftIKWeight = enemyWeapon.IKWeight;
        Transform leftHandTarget = enemyWeapon.LeftHandTarget;
        Transform weapon = enemyWeapon.transform;

        if (leftHandTarget != null)
        {
            UpdateIK(AvatarIKGoal.LeftHand, leftHandTarget, leftIKWeight);
        }

       
        if (isEnemyAiming && currentGunTarget != null)
        {
            Vector3 direction = currentGunTarget.position - weapon.position;
            Quaternion targetRot = Quaternion.LookRotation(direction);

            weapon.rotation = Quaternion.Slerp(weapon.rotation, targetRot, Time.deltaTime * weaponRotateSpeed);
        }
    }

    void UpdateIK(AvatarIKGoal goal, Transform target, float weight)
    {
        if (target == null) return;

        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKRotationWeight(goal, weight);
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation);
    }
}
