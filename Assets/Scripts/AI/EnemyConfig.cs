using UnityEngine;

namespace Akkerman.AI
{
    
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Akkerman/AI/EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("STATS")]
        public float maxHealth = 100f;
        public float speed = 5f;
        public float jumpForce = 10f;
        public float flySpeed = 8f;
        public float detectionRange = 10f;
        public float attackRange = 2f;

        [Header("MODEL")]
        public GameObject modelPrefab;

        [Header("TYPES")]
        public MovementType movementType = MovementType.Ground; // Ground, Flying, Jumping
        public AIType aiType = AIType.Aggressive;

        public enum MovementType { Ground, Flying, Jumping }
        public enum AIType { Aggressive, Scout, Defensive }
    }
}