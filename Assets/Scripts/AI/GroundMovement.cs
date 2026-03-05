using UnityEngine;
using UnityEngine.AI;

namespace Akkerman.AI
{
    public class GroundMovement : MonoBehaviour, IMovement
    {
        protected  NavMeshAgent agent;
        protected float speed;

        public virtual void Initialize(NavMeshAgent navAgent, float speed)
        {
            agent = navAgent;
            this.speed = speed;
            if (agent)
                agent.speed = speed;
        }

        public virtual void MoveTo(Vector3 target)
        {
            if (agent) agent.SetDestination(target);
            else transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        public virtual void Jump(Vector3 direction)
        {
            /* Override in JumpingMovement */
        }

    }
}
