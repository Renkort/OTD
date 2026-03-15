using Akkerman.FPS;
using UnityEngine;

namespace Akkerman.AI
{
    public class JumpAttack : BaseCombat
    {
        Transform attackPoint => enemy.Model.attackPoint;
        PlayerHealth playerHealthCache;
        public override void Initialize(EnemyConfig config, Enemy e)
        {
            base.Initialize(config, e);

        }
        public override void Attack(Vector3 targetPosition)
        {
            Debug.Log("DEBUG: JUMP ATTACK ");
            lastAttackTime = Time.time;
            Collider[] hits = Physics.OverlapSphere(attackPoint.position, config.damageRange, LayerMask.GetMask("Player"));
            foreach (var hit in hits)
            {
                Debug.Log($"DEBUG: hit:{hit.gameObject.name}");
                if (playerHealthCache == null)
                    playerHealthCache = hit.GetComponent<PlayerHealth>();
                playerHealthCache.TakeDamage(config.attackDamage);
            }
            // jump in movement, attack when land
        }
        public override void AttackUpdate(Vector3 targetPosition)
        {
            
        }

        public override void OnAnimEvent(string phase)
        {
            // if (phase == "Land")
            // {
            //     Collider[] hits = Physics.OverlapSphere(enemy.transform.position, config.attackRange, LayerMask.GetMask("Player"));
            //     foreach (var hit in hits)
            //     {
            //         hit.GetComponent<PlayerHealth>()?.TakeDamage(config.attackDamage);
            //         // TODO: VFX over here
            //     }
            // }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, config.damageRange);
        }
    }
}
