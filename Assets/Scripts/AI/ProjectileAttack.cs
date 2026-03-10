using Akkerman.FPS;
using UnityEngine;

namespace Akkerman.AI
{
    public class ProjectileAttack : BaseCombat
    {
        protected Transform attackPoint;
        protected float spreadIntensity;
        protected Vector3 lastTargetPosition;

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
            GameObject projectile = Instantiate(config.projectilePrefab, attackPoint.position, Quaternion.identity);
            Vector3 direction = (targetPosition - projectile.transform.position).normalized;
            direction += CalcSpread();
            
            projectile.transform.forward = direction;
        }
        public override void AttackUpdate(Vector3 targetPosition)
        {
            /* Move shoot point to target */
        }

        public override void OnAnimEvent(string phase)
        {
            /* Shoot animation play */
        }

        private Vector3 CalcSpread()
        {
            float x = Random.Range(-spreadIntensity, spreadIntensity);
            float y = Random.Range(-spreadIntensity, spreadIntensity);
            return new Vector3(x, y, 0.0f);
        }

    }

}
