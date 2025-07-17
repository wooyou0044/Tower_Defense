using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralEntrance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.ReturnEnemy(other.GetComponent<EnemyController>());
        }
    }
}
