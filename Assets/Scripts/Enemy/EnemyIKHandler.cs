using UnityEngine;

public class EnemyIKHandler : MonoBehaviour
{
    private Animator animator;

    public Ak47 enemyWeapon;

    public Transform gunAimTarget;
    public Transform bodyAimTarget;

    [Range(0f, 1f)] public float bodyWeight = 0.5f;
    [Range(0f, 1f)] public float headWeight = 0.5f;
    [Range(0f, 1f)] public float clampWeight = 0.5f;

    public float weaponRotateSpeed = 10f;
    public bool lockYRotation = false;

    private bool playerAiming = false;
    private bool isDead = false; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AimIK(bool aim)
    {
        playerAiming = aim;
    }

    public void Die()
    {
        isDead = true;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || enemyWeapon == null) return;

        if (isDead) return; 

        float leftIKWeight = enemyWeapon.IKWeight;

        Transform leftHandTarget = enemyWeapon.LeftHandTarget;

        if (!isDead && leftHandTarget != null)
        {
            UpdateIK(AvatarIKGoal.LeftHand, leftHandTarget, leftIKWeight);
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
