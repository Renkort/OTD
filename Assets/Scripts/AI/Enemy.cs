using Akkerman.FPS;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Akkerman.AI
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private Transform player;
        private Transform model;

        public EnemyConfig Config => config;
        public Transform Player => player;

        // components:
        private IMovement movement;
        private IAIBrain brain;
        private SensorySystem perceprion;
        private Health health;
        private Animator animator;

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

            health.OnDeath += Die; // TODO: pool

            model = Instantiate(config.modelPrefab, transform).transform;

            animator = model.GetComponent<Animator>();
            if (animator == null) animator = model.gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = config.animatorController;
        }

        private void SetupMovement()
        {
            switch (config.movementType)
            {
                case EnemyConfig.MovementType.Ground:
                    var agent = gameObject.AddComponent<NavMeshAgent>();
                    var rb = gameObject.GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
                    movement = gameObject.AddComponent<GroundMovement>();
                    ((GroundMovement)movement).Initialize(agent, rb, config);
                    break;
                case EnemyConfig.MovementType.Flying:
                    movement = gameObject.AddComponent<FlyingMovement>();
                    ((FlyingMovement)movement).Initialize(config);
                    break;
                case EnemyConfig.MovementType.Jumping:
                    var jumpAgent = gameObject.AddComponent<NavMeshAgent>();
                    // var groundMove = gameObject.AddComponent<GroundMovement>();
                    // groundMove.Initialize(jumpAgent, config.speed);
                    movement = gameObject.AddComponent<JumpingMovement>();
                    var rbJump = gameObject.GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
                    ((JumpingMovement)movement).Initialize(jumpAgent, rbJump, config);
                    ((JumpingMovement)movement).Initialize( config);
                    break;
            }
        }

        private void Update()
        {
            if (health.IsDead) return;

            bool canSeePlayer = perceprion.CanSeeTarget(player);
            // Debug.Log($"DEBUG: Can see player: {canSeePlayer}");
            brain?.UpdateAI(canSeePlayer ? player.position : Vector3.zero);
            movement?.MoveTo(brain?.GetTargetPosition() ?? transform.position);

            if (movement is GroundMovement ground)
                SetMovementSpeed(ground.GetNormalizedSpeed());

        }

        public void SetMovementSpeed(float speed) => animator.SetFloat("Speed", speed, 0.1f, 0.1f); // normalized speed
        public void SetAIState(int stateId) => animator.SetInteger("State", stateId); // 0=Patrol, 1=Chase, 2=Attack
        public void TriggerAction(string trigger) => animator.SetTrigger(trigger);
        public void IsInTransition() => animator.IsInTransition(0);

        public void Die()
        {
            animator.SetTrigger("Death");
            Destroy(gameObject, 3.0f);
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
