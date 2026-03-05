using UnityEngine;

namespace Akkerman.AI
{
    public class SimpleFSM : MonoBehaviour, IAIBrain
    {
        private enum State {Patrol, Chase, Attack}
        [SerializeField] private State currentState = State.Patrol;
        private Vector3 patrolPoint;
        private Enemy enemy;

        private float patrolRange = 0.0f;

        public void Initialize(Enemy e, EnemyConfig.AIType type) // TODO: implement AIType
        {
            enemy = e;
            patrolPoint = transform.position + Random.insideUnitSphere * patrolRange;
        }
        public void UpdateAI(Vector3? playerPosition)
        {
            switch (currentState)
            {
                case State.Patrol:
                    if (playerPosition.HasValue && Vector3.Distance(transform.position, playerPosition.Value) < enemy.Config.detectionRange)
                        currentState = State.Chase;
                    break;
                case State.Chase:
                    if (!playerPosition.HasValue) currentState = State.Patrol;
                    else if (Vector3.Distance(enemy.transform.position, playerPosition.Value) < enemy.Config.attackRange)
                        currentState = State.Attack;
                    break;
                case State.Attack:
                    if (!playerPosition.HasValue || Vector3.Distance(enemy.transform.position, playerPosition.Value) > enemy.Config.attackRange * 2)
                        currentState = State.Chase;

                    // attack animation, VFX, Player damage
                    break;

            }

            enemy.SetAIState((int)currentState);
            if (currentState == State.Attack) enemy.TriggerAction("Attack");
        }

        public Vector3? GetTargetPosition()
        {
            return currentState switch
            {
                State.Patrol => patrolPoint,
                State.Chase or State.Attack => enemy.Player.position,
                _ => null
            };
        }

    }
}
