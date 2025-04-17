using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    [Header("Enemy Layer Only")]
    public LayerMask enemyLayerMask;

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyDamage(RaycastHit hit, float baseDamage)
    {
       
        if (((1 << hit.collider.gameObject.layer) & enemyLayerMask) == 0)
            return;

        if (hit.collider.TryGetComponent<HitPart>(out var hitPart))
        {
            float finalDamage = baseDamage + hitPart.damageValue;
            hitPart.owner.TakeHit(hitPart.partType, finalDamage);
        }
    }
}
