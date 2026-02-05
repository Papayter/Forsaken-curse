using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public EnemyManager enemyManager;
    
    public void SetEnemyManager(EnemyManager manager)
    {
        enemyManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("Player") && !other.CompareTag("Weapon") && !other.CompareTag("EnemyDamage") && !other.CompareTag("Arrow"))
        {
            Destroy(gameObject);
        }
        
    }
}