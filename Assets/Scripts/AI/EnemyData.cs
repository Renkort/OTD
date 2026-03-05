using System.IO;
using UnityEngine;


namespace Akkerman.AI
{
    [CreateAssetMenu(fileName="New Enemy Data", menuName="Akkerman/AI/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("MOVEMENT")]
        public float speed = 3.5f;
        public float chaseRange = 20f;
        public float attackRange = 5f;

        [Header("HEALTH")]
        public float maxHealth = 40.0f;
        public ParticleSystem bloodSplash;

        [Header("ATTACK")]
        public float timeBetweenAttacks = 4f;
        public int damage = 5;

        [Header("SETTINGS")]
        public LayerMask groundLayer = 1;
    }
}
