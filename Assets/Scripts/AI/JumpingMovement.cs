using UnityEngine;
using UnityEngine.AI;


namespace Akkerman.AI
{
    public class JumpingMovement : GroundMovement
    {
        private float jumpAttackRange;
        private float timeBetweenJumps;
        private float jumpForceHorizontal;
        private float jumpForceVertical;
        private float jumpTimer;
        bool isGrounded;
        bool canJump;
        float jumpGroundCheckDelay = 1f;
        float groundCheckTimer;
        Enemy enemy;

        public override void Initialize(NavMeshAgent navAgent, Rigidbody rb, EnemyConfig config)
        {
            base.Initialize(navAgent, rb, config);
        }

        public void Initialize(EnemyConfig config)
        {
            agent.stoppingDistance = config.jumpAttackRange;
            canJump = true;

            jumpForceHorizontal = config.jumpForceHorizontal;
            jumpForceVertical = config.jumpForceVertical;
            jumpAttackRange = config.jumpAttackRange;
            timeBetweenJumps = config.timeBetweenJumps;
            rb.isKinematic = true;

            enemy = GetComponent<Enemy>();
        }

        public override void MoveTo(Vector3 target)
        {
            base.MoveTo(target); // ground movement
            
            jumpTimer -= Time.deltaTime;
            groundCheckTimer -= Time.deltaTime;

            if (!canJump && isGrounded && groundCheckTimer <= 0)
                ResetJump();
            if (CanJump(target))
            {
                rb.isKinematic = false;
                agent.enabled = false;
                canMove = false;
                Jump(target - transform.position);
            }
        }

        public override void Jump(Vector3 direction)
        {
            groundCheckTimer = jumpGroundCheckDelay;
            canJump = false;

            direction = direction.normalized;
            Vector3 jumpVector = new Vector3(direction.x * jumpForceHorizontal, jumpForceVertical,
            direction.z * jumpForceHorizontal);

            enemy.TriggerAction("Jump");

            rb?.AddForce(jumpVector, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            rb.isKinematic = true;
            agent.enabled = true;
            canMove = true;
            canJump = true;
            jumpTimer = timeBetweenJumps;
        }

        private bool CanJump(Vector3 target)
        {
            bool isInRange = Vector3.Distance(transform.position, target) < jumpAttackRange;
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.0f);
            bool isTimeToJump = jumpTimer <= 0;
            // Debug.Log($"DEBUG: inRange:{isInRange}, isGrounded:{isGrounded}, isTime:{isTimeToJump}, canJump:{canJump}");
            return isInRange && isGrounded && isTimeToJump && canJump;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.brown;
            Gizmos.DrawWireSphere(transform.position, 1.0f);
        }
    }
}
