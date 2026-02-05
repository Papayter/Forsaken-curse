using System;
using UnityEngine;
using DG.Tweening;

namespace SI
{
    public class ProjectileMagic : MonoBehaviour
    {
       private PlayerCombat player;
       [SerializeField] private Weapon staff;

       private void Start()
       {
           player = FindObjectOfType<PlayerCombat>();
       }

       private void OnTriggerEnter(Collider other)
       {
           if (!other.CompareTag("Enemy") && !other.CompareTag("Player") && !other.CompareTag("Weapon") && !other.CompareTag("EnemyDamage") && !other.CompareTag("Arrow"))
           {
               transform.DOKill();
                
               player.isProjectileDestroyed = true;
                
               Destroy(gameObject);
           }
       }

       public Weapon GetStaff()
       {
           return staff;
       }
    }
}