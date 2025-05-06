using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    [Header("Enemy Layer ")]
    public LayerMask enemyLayerMask;
    
    [Header("Player Layer ")]
    public LayerMask playerLayerMask;
    
    [Header("Alert Settings")]
    public float alertRadius = 10f; 

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyDamage(RaycastHit hit, float baseDamage)
    {
        int hitLayer = hit.collider.gameObject.layer;
        

        
        if (((1 << hitLayer) & enemyLayerMask) != 0)
        {
            if (hit.collider.TryGetComponent<HitPart>(out var hitPart))
            {
                float finalDamage = baseDamage + hitPart.damageValue;
                hitPart.owner.TakeHit(hitPart.partType, finalDamage,hitPart.damageReductionRate);
                AlertNearbyEnemies(hitPart.owner.transform);
            }
        }
       
        else if (((1 << hitLayer) & playerLayerMask) != 0)
        {
            if (hit.collider.TryGetComponent<HitPart>(out var hitPart))
            {
               
                float finalDamage = baseDamage + hitPart.damageValue;
                hitPart.player.TakeHit(hitPart.partType, finalDamage,hitPart.damageReductionRate);
            }
        }
    }
    private void AlertNearbyEnemies(Transform damagedEnemy)
    {
        
        Collider[] allies = Physics.OverlapSphere(damagedEnemy.position, alertRadius, enemyLayerMask);
        
        foreach (var ally in allies)
        {
            if (ally.GetComponentInParent<EnemyController>() is EnemyController enemy)
            {
                
                if (enemy.transform == damagedEnemy)
                    continue;

                enemy.AllyHit();


            }
        }
    }
    
    public void AlertEnemiesByGunSound(Vector3 origin, float radius)
    {
        Collider[] enemies = Physics.OverlapSphere(origin, radius, enemyLayerMask);
      
        foreach (var col in enemies)
        {
            if (col.GetComponentInParent<EnemyController>() is EnemyController enemy)
            {

                enemy.HeardGunShot();
            }
        }
    }
}
