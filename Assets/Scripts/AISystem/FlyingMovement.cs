using UnityEngine;

namespace Akkerman.AI
{
    public class FlyingMovement : MonoBehaviour, IMovement
    {
        private float flySpeed;
        private float height = 5f;

        public void Initialize(float flySpeed)
        {
            this.flySpeed = flySpeed;
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }
        public void MoveTo(Vector3 target)
        {
            target.y = height;
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
        }

        public void Jump(Vector3 direction)
        {
            throw new System.NotImplementedException();
        }

    }
}
