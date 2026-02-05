using UnityEngine;

namespace SE
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [Header("Enemy Information")]
        public string enemyName;
        public EnemyType enemyType;
        public int health;
        public int damage;
        public float attackRange;
        public GameObject enemyModel;
        /*public ParticleSystem takeDamageEffect;*/
        public AudioClip takeDamageSound;
    }

    public enum EnemyType
    {
        Melee,
        Range
    }
}