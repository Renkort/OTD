using UnityEngine;

namespace Akkerman.AI
{
    public class FlyingMovement : MonoBehaviour, IMovement
    {
        private float flySpeed;
        private float flyHeight;
        private float attackRange;
        Enemy enemy;

        public void Initialize(EnemyConfig config)
        {
            flySpeed = config.flySpeed;
            flyHeight = config.flyHeight;
            attackRange = config.attackRange;

            // transform.position = new Vector3(transform.position.x, flyHeight, transform.position.z);

            enemy = GetComponent<Enemy>();
        }
        public void MoveTo(Vector3 target)
        {
            // target.y = flyHeight;
            flyHeight = target.y;
            Vector3 direction = (target - transform.position).normalized;
            transform.position += direction * flySpeed * Time.deltaTime;

            // Avoidance: from walls and other enemies
            Collider[] nearby = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask("Enemy", "Wall"));
            foreach (var col in nearby)
            {
                if (col.transform != transform)
                    transform.position += (transform.position - col.transform.position).normalized * flySpeed * Time.deltaTime;
            }

            transform.LookAt(target);

            enemy.SetMovementSpeed(GetNormalizedSpeed());
            if (Vector3.Distance(transform.position, target) < attackRange)
                enemy.SetAnimAIState(2); // hover/attack state
        }

        public void Jump(Vector3 direction)
        {
            throw new System.NotImplementedException();
        }

        public float GetNormalizedSpeed() => flySpeed > 0 ? 1f : 0f;

    }
}
