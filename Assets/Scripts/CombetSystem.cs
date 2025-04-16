using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyDamage(RaycastHit hit, float baseDamage)
    {
        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeHit(hit.collider, baseDamage);
        }
    }
}
