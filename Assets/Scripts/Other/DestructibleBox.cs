using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestructibleBox : MonoBehaviour, IDamageable
{
    [Header("Drop Settings")]
    [SerializeField] private GameObject[] possibleDrops = new GameObject[7];
    [SerializeField] private GameObject soundObject;
    
    private int currentHp = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
           AttackEnemy attackEnemy = other.GetComponent<AttackEnemy>();
           PlayerCombat player = FindObjectOfType<PlayerCombat>();
            if (player.IsAttacking())
            {
                TakeDamage(attackEnemy.weapon.damageAmount);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHp -= damageAmount;
        
        if(currentHp <= 0)
            DestroyBox();
    }

    private void DestroyBox()
    {
        GameObject sound = Instantiate(soundObject, transform.position, Quaternion.identity);
        
        SoundManager.Instance.PlayEffects("BoxDestruction", sound.GetComponent<AudioSource>());
        
        if (possibleDrops.Length > 0)
        {
            int randomIndex = Random.Range(0, possibleDrops.Length);
            if (possibleDrops[randomIndex] != null)
            {
                Instantiate(possibleDrops[randomIndex], transform.position, Quaternion.identity);
            }
        }
        
        Destroy(sound, 0.5f);
        Destroy(gameObject);
    }
}