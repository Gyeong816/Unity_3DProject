using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public float hp = 100;
    public float maxHp = 100;
    private float reducedDamage;
    private PlayerController playerController;
    public bool isDead;

    
    private void Start()
    {
        
        playerController = GetComponent<PlayerController>();
    }
    public void TakeHit(Parts part, float damage,float damageReductionRate)
    {
        if (isDead) return;
        switch (part)
        {
            case Parts.Head:
                Debug.Log("Die");
                break;
            case Parts.Vest1:
                reducedDamage = damage * (1f - damageReductionRate);
                TakeDamage(reducedDamage);
                Debug.Log($"방탄복 보호 감소된 데미지 {reducedDamage}, ");
                break;
            case Parts.Vest2:
                reducedDamage = damage * (1f - damageReductionRate);
                TakeDamage(reducedDamage);
                Debug.Log($"방탄복 보호 감소된 데미지 {reducedDamage}, ");
                break;
            case Parts.Vest3:
                reducedDamage = damage * (1f - damageReductionRate);
                TakeDamage(reducedDamage);
                Debug.Log($"방탄복 보호 감소된 데미지 {reducedDamage}, ");
                break;
            default:
                TakeDamage(damage);
                break;
        }
    }
    public void Heal(float amount)
    {
        hp += amount;
        hp = Mathf.Min(hp, maxHp);
        Debug.Log($"회복됨: +{amount}, 현재 체력: {hp}");
    }

    private void TakeDamage(float damage)
    {
        
        hp -= damage;
     
        if (hp <= 0)
        {
            isDead = true;
            StartCoroutine(HandleDeath());
            playerController.OnPlayerDeath();
        }
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(3f);

        
        UIManager.Instance.ShowDeathCanvas();

    }
}
