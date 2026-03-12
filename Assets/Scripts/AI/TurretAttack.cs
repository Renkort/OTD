using System;
using UnityEngine;
using UnityEngine.InputSystem.iOS;


namespace Akkerman.AI
{
    public class TurretAttack : ProjectileAttack
    {
        private float laserRenderRange = 999f;
        private float trackingSpeed = 45f; // degrees in sec
        private float chargeTime = 2.5f;
        private float blinkFrequency = 8f;
        private float maxAimError = 5f;

        private float chargeWidthMin = 0.02f;
        private float chargeWidthMax = 0.08f;
        private Color chargeColor = new Color(1f, 0.2f, 0.2f, 0.7f);
        private Color readyColor  = new Color(1f, 0.1f, 0.1f, 1f);

        private float chargeTimer;
        private bool isCharging;
        private float currentAimError;
        private LineRenderer line => enemy.Model.lineRenderer;
        private Transform head => enemy.Model.head;

        public override void Initialize(EnemyConfig config, Enemy e)
        {
            base.Initialize(config, e);
            chargeTimer = 0.0f;
            isCharging = false;
            currentAimError = 180f;

            ShowLaser(false);
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.enabled = true;
            line.startWidth = line.endWidth = chargeWidthMin;
            line.material.color = chargeColor;
        }

        public override void Attack(Vector3 targetPosition)
        {
            if (isCharging) return;

            isCharging = true;
            chargeTimer = 0.0f;
            lastTargetPosition = targetPosition;
        }

        public override void AttackUpdate(Vector3 targetPosition)
        {
            base.AttackUpdate(targetPosition);
            UpdateLaser();
            // if charging don't move laser
            if (isCharging)
            {
                chargeTimer += Time.deltaTime;
                if (chargeTimer >= chargeTime)
                {
                    base.Attack(lastTargetPosition); // pos when start charging
                    isCharging = false;
                    ShowLaser(false);
                    lastAttackTime = Time.time;
                }
                else
                {
                    UpdateChargingVisual();
                }
                return;
            }


            targetPosition += Vector3.up;
            Vector3 directionToTarget = (targetPosition - enemy.Model.head.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            enemy.Model.head.rotation = Quaternion.RotateTowards(enemy.Model.head.rotation, targetRotation, trackingSpeed * Time.deltaTime);

            currentAimError = Vector3.Angle(head.forward, directionToTarget);
            // OR:
            // enemy.Model.head.rotation = Quaternion.Slerp(
            //     enemy.Model.head.rotation,
            //     targetRotation,
            //     5f * Time.deltaTime   // 5–12 — степень "липкости"
            // );            
        }

        private void UpdateChargingVisual()
        {
            bool on = Math.Sin(Time.time * blinkFrequency * Mathf.PI * 2f) > 0;

            float t = chargeTimer / chargeTime;
            float width = Mathf.Lerp(chargeWidthMin, chargeWidthMax, t);
            line.startWidth = line.endWidth = width;
            line.material.color = Color.Lerp(chargeColor, readyColor, t);

            line.enabled = on;
            // UpdateLaser();
        }

        private void ShowLaser(bool isVisible)
        {
            if (line != null)
            {
                line.positionCount = 2;
                line.useWorldSpace = true;
                line.enabled = true;
                if (!isVisible)
                    line.startWidth = line.endWidth = chargeWidthMin;

                UpdateLaser();
            }
        }


        private void UpdateLaser()
        {
            if (line == null || !line.enabled) return;

            Vector3 hitPoint;

            if (Physics.Raycast(attackPoint.position, attackPoint.forward, out RaycastHit hit, laserRenderRange))
            {
                hitPoint = hit.point;
            }
            else
            {
                hitPoint = attackPoint.position + attackPoint.forward * laserRenderRange;
            }
            enemy.Model.lineRenderer.SetPosition(0, attackPoint.position);
            enemy.Model.lineRenderer.SetPosition(1, hitPoint);
        }

        public override bool CanAttack()
        {
            return currentAimError <= maxAimError && !isCharging && base.CanAttack();
        }
    }
}
