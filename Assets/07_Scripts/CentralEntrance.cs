using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralEntrance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyCtrl = other.GetComponent<EnemyController>();
            GameManager.Instance.ReturnEnemy(enemyCtrl);
            GameManager.Instance.EnemyAttackCentral(enemyCtrl.AttackEnemyAtt());
            Debug.Log("적이 쳐들어옴");
        }
    }
}
