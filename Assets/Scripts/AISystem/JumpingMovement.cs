using UnityEngine;
using UnityEngine.AI;


namespace Akkerman.AI
{
    public class JumpingMovement : GroundMovement
    {
        private Rigidbody rb;
        private float jumpForce;
        private float jumpRange = 5f;

        public override void Initialize(NavMeshAgent navAgent, float speed)
        {
            base.Initialize(navAgent, speed);
            rb = GetComponent<Rigidbody>();
        }

        public void Initialize(Rigidbody rb, float jumpForce)
        {
            this.rb = rb;
            this.jumpForce = jumpForce;
        }

        public override void MoveTo(Vector3 target)
        {
            base.MoveTo(target); // ground movement
            if (Vector3.Distance(transform.position, target) < jumpRange && CanJump())
            {
                Jump(target - transform.position);
            }
        }

        public override void Jump(Vector3 direction)
        {
            rb?.AddForce(direction.normalized * jumpForce, ForceMode.Impulse);
        }

        private bool CanJump() => Physics.Raycast(transform.position, Vector3.down, 1.1f); // TODO: configurate
    }
}
