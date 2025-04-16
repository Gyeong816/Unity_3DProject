using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public List<PartData> hitParts;
    
    public void TakeHit(Collider hitCollider, float baseDamage)
    {
        foreach (var part in hitParts)
        {
            if (part.collider == hitCollider)
            {
                float finalDamage = baseDamage * part.damageValue;
                Debug.Log($"{part.partType} 피격!!, 피해량{finalDamage}");

               
                return;
            }
        }

   
    }
}
