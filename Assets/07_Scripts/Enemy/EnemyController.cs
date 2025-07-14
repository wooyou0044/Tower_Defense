using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyState currentState;

    float currentHP;

    public Animator animator { get; set; }

    void Start()
    {
        ChangeState(new IdleState());
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }

    public void ChangeState(EnemyState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void TakeDamage(int damage)
    {
        animator.SetTrigger("Damage");
        currentHP -= damage;
        if(currentHP <= 0)
        {
            gameObject.SetActive(false);
            GameManager.Instance.ReturnEnemy(this);
        }
    }
}
