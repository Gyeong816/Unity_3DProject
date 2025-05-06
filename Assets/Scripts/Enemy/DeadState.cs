using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IEnemyState
{
    public void EnterState(EnemyController enemy)
    {
        enemy.isDead = true;
        enemy.enemyIKHandler.Die();
        enemy.nav.ResetPath();
        enemy.triggerCollider.enabled = true;
    }

    public void UpdateState(EnemyController enemy) { }
    public void ExitState(EnemyController enemy) { }
}
