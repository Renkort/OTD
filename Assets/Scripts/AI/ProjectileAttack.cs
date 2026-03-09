using Akkerman.FPS;
using UnityEngine;

namespace Akkerman.AI
{
    public class ProjectileAttack : BaseCombat
    {
        protected Transform attackPoint;

        public override void Initialize(EnemyConfig config, Enemy e)
        {
            base.Initialize(config, e);
            config.projectilePrefab.GetComponent<Bullet>().Initialize(config.attackDamage);
            attackPoint = e.Model.attackPoint;
        }
        public override void Attack(Vector3 targetPosition)
        {
            lastAttackTime = Time.time;
            GameObject projectile = Instantiate(config.projectilePrefab, attackPoint.position, Quaternion.identity);
            Vector3 direction = (targetPosition - projectile.transform.position).normalized;
            // projectile.GetComponent<Rigidbody>().linearVelocity = direction * config.projectileSpeed;
            projectile.transform.forward = direction;
            // OnTriggerEnter -> Damage player (takeDamage)
        }
        public override void AttackUpdate(Vector3 targetPosition)
        {
            /* Move shoot point to target */
        }

        public override void OnAnimEvent(string phase)
        {
            /* Shoot animation play */
        }

    }

}
