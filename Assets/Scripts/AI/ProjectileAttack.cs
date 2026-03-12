using System.Collections;
using Akkerman.FPS;
using UnityEngine;

namespace Akkerman.AI
{
    public class ProjectileAttack : BaseCombat
    {
        protected Transform attackPoint;
        protected float spreadIntensity;
        protected Vector3 lastTargetPosition;
        protected int burst = 10;
        protected int shootRate = 5; // shots per second

        public override void Initialize(EnemyConfig config, Enemy e)
        {
            base.Initialize(config, e);
            config.projectilePrefab.GetComponent<Bullet>().Initialize(config.attackDamage);
            attackPoint = e.Model.attackPoint;
            spreadIntensity = config.shootSpread;
        }
        public override void Attack(Vector3 targetPosition)
        {
            lastAttackTime = Time.time;

            StartCoroutine(ShootSequence(targetPosition));            
        }
        public override void AttackUpdate(Vector3 targetPosition)
        {
            /* Move shoot point to target */
        }

        public override void OnAnimEvent(string phase)
        {
            /* Shoot animation play */
        }

        private IEnumerator ShootSequence(Vector3 targetPosition)
        {
            for (int i = 0; i < burst; i++)
            {
                GameObject projectile = Instantiate(config.projectilePrefab, attackPoint.position, Quaternion.identity);
                Vector3 direction = (targetPosition - projectile.transform.position).normalized;
                direction += CalcSpread();
                projectile.transform.forward = direction;

                yield return new WaitForSeconds(1f / shootRate);
            }
        }

        private Vector3 CalcSpread()
        {
            float x = Random.Range(-spreadIntensity, spreadIntensity);
            float y = Random.Range(-spreadIntensity, spreadIntensity);
            return new Vector3(x, y, 0.0f);
        }

    }

}
