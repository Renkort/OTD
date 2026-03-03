using Akkerman.FPS;
using UnityEngine;
using UnityEngine.AI;

namespace Akkerman.AI
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private Transform player;
        [SerializeField] private Transform modelParent;

        public EnemyConfig Config => config;
        public Transform Player => player;

        // components:
        private IMovement movement;
        private IAIBrain brain;
        private SensorySystem perceprion;
        private Health health;

        void Awake()
        {
            player = FindAnyObjectByType<Player>().transform;
            if (player == null)
                Debug.LogError("ERROR: No Player found");
        }

        public void Initialize(EnemyConfig config)
        {
            this.config = config;
            health = gameObject.AddComponent<Health>();
            health.Initialize(config.maxHealth);
            perceprion = gameObject.AddComponent<SensorySystem>();
            perceprion.Initialize(config.detectionRange, LayerMask.GetMask("Player"));

            SetupMovement();

            brain = gameObject.AddComponent<SimpleFSM>();
            ((SimpleFSM)brain).Initialize(this, config.aiType);

            health.OnDeath += () => Destroy(gameObject); // or pool
            
            Instantiate(config.modelPrefab, modelParent);


        }

        private void SetupMovement()
        {
            switch (config.movementType)
            {
                case EnemyConfig.MovementType.Ground:
                    var agent = gameObject.AddComponent<NavMeshAgent>();
                    agent.speed = config.speed;
                    movement = gameObject.AddComponent<GroundMovement>();
                    ((GroundMovement)movement).Initialize(agent, config.speed);
                    break;
                case EnemyConfig.MovementType.Flying:
                    movement = gameObject.AddComponent<FlyingMovement>();
                    ((FlyingMovement)movement).Initialize(config.flySpeed);
                    break;
                case EnemyConfig.MovementType.Jumping:
                    var rb = gameObject.GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
                    var groundMove = gameObject.AddComponent<GroundMovement>();
                    groundMove.Initialize(null, config.speed); // TODO: use nav mesh agent
                    movement = gameObject.AddComponent<JumpingMovement>();
                    ((JumpingMovement)movement).Initialize(rb, config.jumpForce);
                    break;
            }
        }

        private void Update()
        {
            if (health.IsDead) return;
            bool canSeePlayer = perceprion.CanSeeTarget(player);
            Debug.Log($"DEBUG: Can see player: {canSeePlayer}");
            brain?.UpdateAI(canSeePlayer ? player.position : Vector3.zero);
            movement?.MoveTo(brain?.GetTargetPosition() ?? transform.position);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, config.detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, config.attackRange);
        }
    }
    
}
