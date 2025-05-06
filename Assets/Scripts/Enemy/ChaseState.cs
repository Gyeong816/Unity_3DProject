using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private float forceChaseTimer = 0f;
    private float shootTimer = 0f;
    
    public void EnterState(EnemyController enemy)
    {
        Debug.Log("Entering ChaseState");
        enemy.animator.SetBool("Move", true);
        enemy.nav.ResetPath();
        enemy.LookAtPlayer();
        enemy.nav.stoppingDistance = enemy.stopDistance;
        enemy.nav.SetDestination(enemy.player.position);
        shootTimer = 0f; 
        forceChaseTimer = enemy.forceChaseDuration;
        enemy.lostSightTimer = 0f; 
    }

    public void UpdateState(EnemyController enemy)
    {
        if (enemy.playerHP.isDead)
        {
            enemy.nav.ResetPath();
            enemy.enemyIKHandler.AimIK(false); 
            enemy.ChangeState(new PatrolState()); 
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);
       
        
        if (forceChaseTimer > 0f)
        {
            forceChaseTimer -= Time.deltaTime;
        }

        
        if (enemy.CanSeePlayer())
        {
            enemy.lastKnownPlayerPosition = enemy.player.position;
            
            enemy.lostSightTimer = 0f;
            enemy.LookAtPlayer();
            
            enemy.enemyIKHandler.AimIK(true);
            shootTimer -= Time.deltaTime;
            
            if (shootTimer <= 0f)
            {
                shootTimer = enemy.shootInterval;
                enemy.FireWeapon(); 
            }
            
            
            if (distance > enemy.stopDistance)
            {
                enemy.animator.SetBool("AimStop", false);
                enemy.nav.stoppingDistance = enemy.stopDistance;
                enemy.nav.SetDestination(enemy.player.position);
            }
            else if(distance < enemy.stopDistance)
            {
                enemy.animator.SetBool("AimStop", true);
                enemy.nav.ResetPath();
            
            }
        }
        else
        {
            enemy.enemyIKHandler.AimIK(false);

            
            if (enemy.IsPlayerBehind()||enemy.CanSeePlayer())
            {
                enemy.LookAtPlayer();
                return;
            }

            

            
            enemy.nav.stoppingDistance = 0f; 
            enemy.nav.SetDestination(enemy.lastKnownPlayerPosition);

            
            if (!enemy.nav.pathPending && enemy.nav.remainingDistance <= enemy.arrivalThreshold)
            {
                enemy.nav.ResetPath();
                enemy.animator.SetBool("AimStop", true);

                if (!enemy.reachedLastPosition)
                {
                    enemy.reachedLastPosition = true;
                    enemy.waitAfterLostTimer  = 0f;
                }

                enemy.waitAfterLostTimer += Time.deltaTime;
                if (enemy.waitAfterLostTimer >= enemy.waitAfterLostTime &&
                    !enemy.CanSeePlayer())
                {
                    enemy.animator.SetBool("AimStop", false);
                    enemy.reachedLastPosition = false;
                    enemy.ChangeState(new PatrolState());
                }
            }
            else
            {
                enemy.reachedLastPosition = false;
            }
        }
        

    }

    public void ExitState(EnemyController enemy) { }
}

