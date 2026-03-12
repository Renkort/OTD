using Akkerman.FPS;
using UnityEngine;
using UnityEngine.AI;

namespace Akkerman.AI
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private Transform player;
        private EnemyModel model;

        public EnemyConfig Config => config;
        public Transform Player => player;
        public EnemyModel Model => model;

        // components:
        private IMovement movement;
        private ICombat combat;
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
            health.OnDeath += Die; // TODO: pool
            perceprion = gameObject.AddComponent<SensorySystem>();
            perceprion.Initialize(config.detectionRange, LayerMask.GetMask("Player", "Static"));

            brain = gameObject.AddComponent<SimpleFSM>();
            ((SimpleFSM)brain).Initialize(this, config.aiType);

            model = Instantiate(config.modelPrefab, transform).GetComponent<EnemyModel>();
            if (model == null)
                Debug.LogError("ERROR: Null referece exception: No EnemyModel component");
            animator = model.transform.GetComponent<Animator>();
            if (animator == null) animator = model.gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = config.animatorController;

            SetupMovement();
            SetupCombat();
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

        private void SetupCombat()
        {
            switch (config.combatType)
            {
                case EnemyConfig.CombatType.Melee:
                    combat = gameObject.AddComponent<MeleeAttack>();
                    ((MeleeAttack)combat).Initialize(config, this);
                    break;
                case EnemyConfig.CombatType.Projectile:
                    combat = gameObject.AddComponent<ProjectileAttack>();
                    ((ProjectileAttack)combat).Initialize(config, this);
                    break;
                case EnemyConfig.CombatType.JumpDamage:
                    combat = gameObject.AddComponent<JumpAttack>();
                    ((JumpAttack)combat).Initialize(config, this);
                    break;
                case EnemyConfig.CombatType.Turret:
                    combat = gameObject.AddComponent<TurretAttack>();
                    ((TurretAttack)combat).Initialize(config, this);
                    break;
            }
            // if (combat != null)
                // ((MonoBehaviour)combat).Initiazlize
        }

        private void Update()
        {
            if (health.IsDead) return;

            bool canSeePlayer = perceprion.CanSeeTarget(player);
            // Debug.Log($"DEBUG: Can see player: {canSeePlayer}");
            brain.UpdateAI(canSeePlayer ? player.position : Vector3.zero);
            movement.MoveTo(brain?.GetTargetPosition() ?? transform.position);

            if (movement is GroundMovement ground)
                SetMovementSpeed(ground.GetNormalizedSpeed());

            if (brain.GetCurrentState() == 2 && canSeePlayer) // attack state
                combat.AttackUpdate(player.position);
            
            if (brain.GetCurrentState() == (int)SimpleFSM.State.Attack && combat.CanAttack())
            {
                combat.Attack(player.position);
                TriggerAction("Attack");
            }
        }

        public void SetMovementSpeed(float speed) => animator.SetFloat("Speed", speed, 0.1f, 0.1f); // normalized speed
        public void SetAnimAIState(int stateId) => animator.SetInteger("State", stateId); // 0=Patrol, 1=Chase, 2=Attack
        public void TriggerAction(string trigger) => animator.SetTrigger(trigger);
        public void IsInTransition() => animator.IsInTransition(0);
        public ICombat GetCombat() => combat;
        public float GetAttackRange() => combat?.GetRange() ?? config.attackRange;

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
