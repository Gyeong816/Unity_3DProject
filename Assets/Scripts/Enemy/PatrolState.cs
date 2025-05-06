using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    
    private bool isWaitingIdle = false;
    public void EnterState(EnemyController enemy)
    {
        Debug.Log("Entering PatrolState");
        enemy.animator.SetBool("AimStop", false);
        enemy.animator.SetBool("Move", true);
        
        isWaitingIdle = false;

        if (enemy.patrolPoints.Length > 0)
            enemy.nav.SetDestination(enemy.patrolPoints[enemy.currentIndex].position);
    }
    

    public void UpdateState(EnemyController enemy)
    {
        
        if (enemy.CanSeePlayer()|| enemy.IsPlayerBehind())
        {
            enemy.ChangeState(new ChaseState());
            return;
        }
        

        
        
        if (!enemy.nav.pathPending && enemy.nav.remainingDistance <= enemy.arrivalThreshold)
        {
            if (!isWaitingIdle)
            {
                enemy.nav.ResetPath();
                enemy.animator.SetBool("Move", false);
                isWaitingIdle = true;
            }

            AnimatorStateInfo stateInfo = enemy.animator.GetCurrentAnimatorStateInfo(0);

            if (isWaitingIdle && stateInfo.IsName("Idle") && stateInfo.normalizedTime >= 1.0f)
            {

                int nextIndex;
                do
                {
                    nextIndex = Random.Range(0, enemy.patrolPoints.Length);
                }
                while (nextIndex == enemy.currentIndex);

                enemy.currentIndex = nextIndex;
                enemy.nav.SetDestination(enemy.patrolPoints[enemy.currentIndex].position);
                enemy.animator.SetBool("Move", true);
                isWaitingIdle = false;
            }
        }
        

    }
    

    public void ExitState(EnemyController enemy)
    {
        isWaitingIdle = false;
    }
}
