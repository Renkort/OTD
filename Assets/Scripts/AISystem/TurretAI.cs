using UnityEngine;
using System.Collections;
using Akkerman.FPS;

namespace Akkerman.AI
{
    public class TurretAI : MonoBehaviour, IDamagable
    {
        [Header("References")]
        [SerializeField] private Transform head;
        [SerializeField] private Transform muzzle;
        [SerializeField] private LineRenderer laserLine;
        [SerializeField] private Transform laserPoint;
        [SerializeField] private string playerTag = "Player";

        [Header("Settings")]
        [SerializeField] private bool isOn = true;
        [SerializeField] private float visionRange = 15f;
        [SerializeField] private float aimTime = 3f;
        [SerializeField] private float timeBeforeShoot = 0.6f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private int damage = 40;
        // [SerializeField] private float laserDuration = 0.2f;
        [SerializeField] private bool canTakeDamage = true;
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth = 80;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private GameObject hitVFXPrefab;

        [Header("Idle Behavior")]
        [SerializeField] private Transform idleTargetPoint;
        [SerializeField] private float idleRotationSpeed = 30f;


        [Header("DEBUG")]
        [SerializeField] private bool isSeePlayer;
        [SerializeField] private bool isPlayerInRange;

        private Transform player;
        private float aimTimer = 0f;
        private float fireCooldown = 0f;
        // private bool isAiming = false;
        // private bool isFiring = false;

        private enum TurretState { Idle, Aiming, Firing }
        private TurretState currentState = TurretState.Idle;

        void Start()
        {
            player = FPS.Player.Instance.gameObject.transform;

            currentHealth = maxHealth;
            if (laserLine != null)
                laserLine.useWorldSpace = true;

            if (isOn)
                ShowLaserLine();
        }

        void Update()
        {
            if (!isOn)
                return;

            if (fireCooldown > 0)
                fireCooldown -= Time.deltaTime;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            bool playerInRange = distanceToPlayer <= visionRange;

            isPlayerInRange = playerInRange;
            isSeePlayer = CanSeePlayer();

            switch (currentState)
            {
                case TurretState.Idle:
                    IdleBehavior();
                    if (playerInRange && CanSeePlayer())
                    {
                        currentState = TurretState.Aiming;
                        aimTimer = 0f;
                        // isAiming = true;
                    }
                    break;

                case TurretState.Aiming:
                    AimingBehavior();
                    aimTimer += Time.deltaTime;

                    if (!playerInRange || !CanSeePlayer())
                    {
                        currentState = TurretState.Idle;
                        // isAiming = false;
                        break;
                    }

                    if (aimTimer >= aimTime)
                    {
                        currentState = TurretState.Firing;
                        Fire();
                    }
                    break;

                case TurretState.Firing:
                    if (fireCooldown <= 0)
                    {
                        currentState = TurretState.Aiming;
                        aimTimer = 0f;
                    }
                    break;
            }

            UpdateLaser();
        }

        public void Activate(bool isActive)
        {
            isOn = isActive;
            laserLine.enabled = isActive;
        }

        public void TakeDamage(int damage)
        {
            if (!canTakeDamage)
                return;
            currentHealth -= damage;
            GameObject effect = Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);

            Destroy(effect, 0.2f);
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        private void Die()
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity, transform);

            Destroy(gameObject, explosion.GetComponent<ParticleSystem>().main.duration - 0.5f);
        }

        private void IdleBehavior()
        {
            // Плавно поворачиваем голову к точке покоя
            Quaternion targetRotation = Quaternion.LookRotation(idleTargetPoint.position - head.position);
            head.rotation = Quaternion.RotateTowards(head.rotation, targetRotation, idleRotationSpeed * Time.deltaTime);
        }

        private void AimingBehavior()
        {
            // Поворачиваем голову к игроку
            Vector3 directionToPlayer = (player.position - head.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            head.rotation = Quaternion.RotateTowards(head.rotation, targetRotation, 360 * Time.deltaTime);
        }

        private void Fire()
        {
            if (fireCooldown > 0) return;

            StartCoroutine(FireSequence());
            fireCooldown = 1f / fireRate;
        }

        IEnumerator FireSequence()
        {
            // isFiring = true;

            if (laserLine != null)
            {
                Color originalColor = laserLine.startColor;
                laserLine.startColor = Color.red;
                laserLine.endColor = Color.red;

                int blinkCout = (int)(timeBeforeShoot / 0.1f);
                for (int i = 0; i < blinkCout; i++)
                {
                    laserLine.enabled = false;
                    yield return new WaitForSeconds(0.1f);
                    laserLine.enabled = true;
                    yield return new WaitForSeconds(0.1f);
                }

                laserLine.startColor = originalColor;
                laserLine.endColor = originalColor;
            }

            // 2. Визуальный эффект выстрела (опционально)
            // Здесь можно добавить Particle System или вспышку

            // 3. Логика нанесения урона/выстрела
            RaycastHit hit;
            if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, visionRange))
            {
                if (hit.collider.tag == playerTag)
                {
                    player.GetComponent<PlayerHealth>().TakeDamage(damage);
                }
            }

            // isFiring = false;
        }

        private void ShowLaserLine()
        {
            if (laserLine != null)
            {
                laserLine.positionCount = 2;
                laserLine.useWorldSpace = true;
                laserLine.enabled = true;
            }

            if (idleTargetPoint.position == Vector3.zero)
                idleTargetPoint.position = muzzle.position + muzzle.forward * 10f;
        }

        private void UpdateLaser()
        {
            if (laserLine == null) return;

            RaycastHit hit;
            Vector3 hitPoint;

            if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, visionRange))
            {
                hitPoint = hit.point;
            }
            else
            {
                hitPoint = muzzle.position + muzzle.forward * visionRange;
            }

            laserLine.SetPosition(0, muzzle.position);
            laserLine.SetPosition(1, hitPoint);

            if (laserPoint != null)
            {
                laserPoint.position = hitPoint;
            }
        }

        private bool CanSeePlayer()
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (player.position - muzzle.position).normalized;

            if (Physics.Raycast(muzzle.position, directionToPlayer, out hit, visionRange))
            {
                return hit.collider.tag == playerTag;
            }
            return false;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, visionRange);

            Gizmos.color = Color.green;
            if (muzzle != null && idleTargetPoint != null)
                Gizmos.DrawLine(muzzle.position, idleTargetPoint.position);
        }
    }
}