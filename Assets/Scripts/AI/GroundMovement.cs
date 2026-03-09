using UnityEngine;
using UnityEngine.AI;

namespace Akkerman.AI
{
    public class GroundMovement : MonoBehaviour, IMovement
    {
        protected Rigidbody rb;
        protected  NavMeshAgent agent;
        protected float speed;
        protected bool canMove = true;

        public virtual void Initialize(NavMeshAgent navAgent, Rigidbody rb, EnemyConfig config)
        {
            agent = navAgent;
            this.rb = rb;
            speed = config.speed;
            if (agent)
                agent.speed = speed;
            agent.height = config.agentHeight;
            rb.mass = config.mass;
            rb.isKinematic = true;
        }

        public virtual void MoveTo(Vector3 target)
        {
            if (!canMove)
                return;

            if (agent && agent.enabled) agent.SetDestination(target);
            else transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        public virtual void Jump(Vector3 direction)
        {
            /* Override in JumpingMovement */
        }


        public float GetNormalizedSpeed() => agent ? agent.velocity.magnitude / speed : speed;

    }
}
