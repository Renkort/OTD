using UnityEditor.Animations;
using UnityEngine;

namespace Akkerman.AI
{
    
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Akkerman/AI/EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("STATS")]
        public float maxHealth = 100f;
        public float speed = 5f;
        public float agentHeight = 1.8f;
        public float jumpForceHorizontal = 20f;
        public float jumpForceVertical = 35f;
        public float jumpAttackRange = 5f;
        public float timeBetweenJumps = 4f;
        public float mass = 75f;
        public float flySpeed = 8f;
        public float flyHeight = 5f;
        public float detectionRange = 10f;

        [Header("COMBAT")]
        public CombatType combatType = CombatType.Melee;
        public float attackDamage = 25f;
        public float attackRange = 2f;
        public float attackCooldown = 1.5f;
        public float shootSpread = 0.02f;
        public GameObject projectilePrefab;

        [Header("MODEL")]
        public GameObject modelPrefab;
        public AnimatorController animatorController;

        [Header("TYPES")]
        public MovementType movementType = MovementType.Ground; // Ground, Flying, Jumping
        public AIType aiType = AIType.Aggressive;

        public enum MovementType { Ground, Flying, Jumping }
        public enum AIType { Aggressive, Scout, Defensive }
        public enum CombatType { Melee, Projectile, JumpDamage, Turret }
    }
}