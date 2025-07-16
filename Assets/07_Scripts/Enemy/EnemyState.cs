using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState
{
    public virtual void EnterState(EnemyController enemy) { }
    public virtual void UpdateState(EnemyController enemy) { }
    public virtual void FixedUpdateState(EnemyController enemy) { }
}

public class IdleState : EnemyState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.animator.SetBool("isMove", false);
        enemy.animator.SetBool("isAttack", false);
    }

    public override void UpdateState(EnemyController enemy)
    {
    }

    public override void FixedUpdateState(EnemyController enemy)
    {
    }
}

public class MoveState : EnemyState
{
    public override void EnterState(EnemyController enemy)
    {
        Debug.Log("들어왔음! 움직여야 함");
        enemy.animator.SetBool("isMove", true);
    }

    public override void UpdateState(EnemyController enemy)
    {
        Debug.Log("움직이고 있음");
        enemy.MovePath();
    }

    public override void FixedUpdateState(EnemyController enemy)
    {
    }
}

public class AttackState : EnemyState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.animator.SetBool("isAttack", true);
    }

    public override void UpdateState(EnemyController enemy)
    {
    }
    public override void FixedUpdateState(EnemyController enemy)
    {
    }
}
