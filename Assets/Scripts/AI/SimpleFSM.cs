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
                    float distToPlayer = Vector3.Distance(transform.position, playerPosition.Value);
                    // Debug.Log($"DEBUG: Dist to Player: {distToPlayer}");
                    // Debug.Log($"DEBUG: Dist less than detect range: {distToPlayer < enemy.Config.detectionRange}");
                    // Debug.Log($"DEBUG: Player pos HasValue: {playerPosition.HasValue}");
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
