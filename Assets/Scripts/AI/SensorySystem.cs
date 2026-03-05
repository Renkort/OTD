using UnityEngine;


namespace Akkerman.AI
{
    public class SensorySystem : MonoBehaviour
    {
        public float range;
        public LayerMask targetMask;
        private Vector3 targetDirection;

        public void Initialize(float r, LayerMask mask)
        {
            range = r;
            targetMask = mask;
        }

        public bool CanSeeTarget(Transform target)
        {
            if (target == null || Vector3.Distance(transform.position, target.position) > range )
                return false;
            targetDirection = (target.position - transform.position).normalized;
            return Physics.Raycast(transform.position, targetDirection, range, targetMask);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.pink;
            Gizmos.DrawRay(transform.position, targetDirection);
        }
    }
}
