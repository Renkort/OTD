using UnityEngine;


namespace Akkerman.AI
{
    public class TurretAttack : ProjectileAttack
    {
        private float laserRenderRange = 999f;
        public override void Attack(Vector3 targetPosition)
        {
            enemy.Model.head.LookAt(targetPosition);
            base.Attack(targetPosition);
        }

        public override void AttackUpdate(Vector3 targetPosition)
        {
            base.AttackUpdate(targetPosition);

            UpdateLaser();

            Vector3 directionToTarget = (targetPosition - enemy.Model.head.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            enemy.Model.head.localRotation = Quaternion.RotateTowards(enemy.Model.head.localRotation, targetRotation, 360 * Time.deltaTime);
        }


        private void UpdateLaser()
        {
            if (enemy.Model.lineRenderer == null) return;

            RaycastHit hit;
            Vector3 hitPoint;

            if (Physics.Raycast(attackPoint.position, attackPoint.forward, out hit, 999))
                hitPoint = hit.point;
            else
                hitPoint = attackPoint.position + attackPoint.forward * laserRenderRange;
            enemy.Model.lineRenderer.SetPosition(0, attackPoint.position);
            enemy.Model.lineRenderer.SetPosition(1, hitPoint);
        }
    }
}
