using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private static readonly int HEADSHOT = Animator.StringToHash("Headshot");
    private static readonly int DIE = Animator.StringToHash("Die");
    
    public Animator animator;
    
    
    public float hp = 100;
    
    
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
