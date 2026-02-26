using UnityEngine;
using UnityEngine.AI;
using Akkerman.FPS;
using NUnit.Framework;

namespace Akkerman.AI
{
    public class MonsterAI : MonoBehaviour, IDamagable
    {
        [Header("SETTINGS")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private float jumpForceHorizontal = 20f;
        [SerializeField] private float jumpForceVertical = 35f;
        private bool isDead = false;

        [Header("COMPONENTS")]
        [SerializeField] private Collider damageCollider;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Rigidbody rb; // mass = 5
        [SerializeField] private Animator animator;
        private int currentHealth;
        private float attackTimer;
        private bool isJumping = false;
        [SerializeField] private bool grounded;
        [SerializeField] private MonsterState currentState = MonsterState.Chase;

        public enum MonsterState
        {
            Chase,
            JumpAttack,
            Dead
        }

        void Start()
        {
            currentHealth = enemyData.maxHealth;
            agent.stoppingDistance = enemyData.attackRange;
            agent.speed = enemyData.speed;

            damageCollider.enabled = false;
            if (player == null) player = GameObject.FindWithTag("Player").transform;
            attackTimer = enemyData.timeBetweenAttacks;
        }

        void Update()
        {
            grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.0f, enemyData.groundLayer);
            attackTimer -= Time.deltaTime;

            switch (currentState)
            {
                case MonsterState.Chase:
                    ChaseState();
                    break;
                case MonsterState.JumpAttack:
                    JumpAttackState();
                    break;
                case MonsterState.Dead:
                    DeadState();
                    break;
            }

            if (animator) animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        void ChaseState()
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer < enemyData.chaseRange)
            {

                if (distToPlayer < enemyData.attackRange && grounded && attackTimer <= 0)
                {
                    Debug.Log("DEBUG: [1] JUMP STATE");
                    currentState = MonsterState.JumpAttack;
                    if (animator) animator.SetTrigger("Jump");
                }
                else
                {
                    agent.SetDestination(player.position);
                    
                }
            }
            else
            {
                // idle state or patrol
                agent.ResetPath();
            }
        }

        void DeadState()
        {
            if (!isDead)
                Die();
        }

        void Die()
        {
            isDead = true;
            agent.enabled = false;
            damageCollider.enabled = false;

            animator.SetTrigger("Die");
            Instantiate(enemyData.bloodSplash, gameObject.transform.position, Quaternion.identity);
            //bloodSplash.transform.position = transform.position + Vector3.up;
            // bloodSplash.Play();
            Destroy(gameObject, 2.0f);
        }

        void JumpAttackState()
        {
            agent.enabled = false;
            rb.isKinematic = false;

            if (!isJumping)
                Jump();
            else if (isJumping && grounded && attackTimer <= 0)
                ResetJump();
        }

        void Jump()
        {
            Debug.Log("DEBUG: [2] JUMP!!!");
            isJumping = true;
            damageCollider.enabled = true;
            
            attackTimer = enemyData.timeBetweenAttacks;

            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            Vector3 jumpVector = new Vector3(dirToPlayer.x * jumpForceHorizontal, jumpForceVertical, dirToPlayer.z * jumpForceHorizontal);

            rb.AddForce(jumpVector, ForceMode.Impulse);
            //Invoke(nameof(ResetJump), enemyData.timeBetweenAttacks);
            
        }

        void ResetJump()
        {
            Debug.Log("DEBUG: [3] RESET JUMP");
            isJumping = false;
            damageCollider.enabled = false;
            agent.enabled = true;
            rb.isKinematic = true;
            // rb.linearVelocity = Vector3.zero;
            currentState = MonsterState.Chase;
            // attackTimer = enemyData.timeBetweenAttacks;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null) playerHealth.TakeDamage(enemyData.damage);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == enemyData.groundLayer.value)
            {
                CancelInvoke(nameof(ResetJump));
                ResetJump();
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                currentState = MonsterState.Dead;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gameObject.transform.position, enemyData.attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(gameObject.transform.position, enemyData.chaseRange);
        }
    }
}
