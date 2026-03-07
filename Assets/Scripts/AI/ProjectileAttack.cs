using UnityEngine;

namespace Akkerman.AI
{
    public class ProjectileAttack : BaseCombat
    {
        public override void Attack(Vector3 targetPosition)
        {
            lastAttackTime = Time.time;
            GameObject projectile = Instantiate(config.projectilePrefab, enemy.transform.position + Vector3.up, Quaternion.identity);
            Vector3 direction = (targetPosition - projectile.transform.position).normalized;
            projectile.GetComponent<Rigidbody>().linearVelocity = direction * config.projectileSpeed;
            // OnTriggerEnter -> Damage player (takeDamage)
        }

        public override void OnAnimEvent(string phase)
        {
            /* Shoot animation play */ 
        }
    }

}
