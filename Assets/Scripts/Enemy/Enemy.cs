using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{ 
    private static readonly int HEADSHOT = Animator.StringToHash("Headshot");
    private static readonly int DIE = Animator.StringToHash("Die");
    
    public Animator animator;
    public float hp = 100;
    public Vest vest;
    
    
    private bool isDead = false;
    
    
    
   

   
    public void TakeHit(Parts part, float damage)
    {
        if(isDead) return;
        
        switch (part)
        {
            case Parts.Head:
                Debug.Log(" Head Hit");
                HeadShotDie();
                break;
            case Parts.Body:
                Debug.Log("Body Hit");
                TakeDamage(damage);
                break;
            case Parts.Arm:
                Debug.Log("Arm Hit");
                TakeDamage(damage);
                break;
            case Parts.Leg:
                Debug.Log("Leg Hit");
                TakeDamage(damage);
                break;
            case Parts.Helmet:
                Debug.Log("Helmet Hit");
                TakeDamage(damage);
                break;
            case Parts.Vest:
                Debug.Log("Vest Hit");
                float reduceddamage = damage * (1f - vest.damageReductionRate);
                vest.durability -= reduceddamage;
                Debug.Log($"[방탄복 보호] 피해 감소됨: {reduceddamage}, 방탄복 내구도 {vest.durability}");
                
                TakeDamage(reduceddamage);
                break;
            default:
                break;
        }
        
    }

    private void HeadShotDie()
    {
        animator.SetTrigger(HEADSHOT);
        isDead = true;
    }

    private void Die()
    {
        animator.SetTrigger(DIE);
        isDead = true;
    }
    
    private void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log($"데미지 {damage},  남은 hp{hp}");
        
        if (hp <= 0)
        {
            Die();
        }
    }
    
}
