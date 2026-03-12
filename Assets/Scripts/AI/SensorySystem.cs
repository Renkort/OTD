using UnityEngine;


namespace Akkerman.AI
{
    public class SensorySystem : MonoBehaviour
    {
        public float range;
        public LayerMask targetMask;
        private Vector3 targetDirection;
        public bool canSeeTarget;

        public void Initialize(float r, LayerMask mask)
        {
            range = r;
            targetMask = mask;
            canSeeTarget = false;
        }

        public bool CanSeeTarget(Vector3 target)
        {
            if (target == null || Vector3.Distance(transform.position, target) > range)
                return canSeeTarget = false;
            targetDirection = (target - transform.position).normalized;

            if (Physics.Raycast(transform.position, targetDirection, out RaycastHit hit, range/*, targetMask*/))
            {
                canSeeTarget = (1 << hit.collider.gameObject.layer) == targetMask;
            }
            return canSeeTarget;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.pink;
            Gizmos.DrawRay(transform.position, targetDirection);
        }
    }
}
