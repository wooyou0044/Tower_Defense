using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    EnemyState currentState;
    public List<Vector3> movePath { get; set; }
    public bool isMoving { get; set; }
    public int currentIndex { get; set; }
    float currentHP;
    Collider col;

    public Animator animator { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
    }

    void Start()
    {
        //ChangeState(new IdleState());
    }

    void Update()
    {
        if (isMoving == false || movePath == null || currentIndex >= movePath.Count)
        {
            return;
        }
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

    public void SetPath(List<Vector3> path)
    {
        movePath = path;
        currentIndex = 0;
        isMoving = true;

        ChangeState(new MoveState());
    }

    public void MovePath()
    {
        Vector3 targetPos = movePath[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        transform.LookAt(targetPos);
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentIndex++;

            if (currentIndex >= movePath.Count)
            {
                isMoving = false;
            }
        }
    }

    public void SeperateNearbyEnemies(float radius = 1f, float pushPower = 2f)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach(var hit in hits)
        {
            if(hit.gameObject == this || hit.CompareTag("Enemy") == false)
            {
                continue;
            }

            Vector3 dir = transform.position - hit.transform.position;
            float distance = dir.magnitude;
            if (distance < 0.01f)
            {
                continue;
            }

            float pushStrength = pushPower * (1f - (distance / radius));
            transform.position += dir.normalized * pushStrength * Time.deltaTime;
        }
    }
}
