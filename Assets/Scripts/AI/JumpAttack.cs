using Akkerman.FPS;
using UnityEngine;

namespace Akkerman.AI
{
    public class JumpAttack : BaseCombat
    {
        public override void Attack(Vector3 targetPosition)
        {
            lastAttackTime = Time.time;
            // jump in movement, attack when land
        }
        public override void AttackUpdate(Vector3 targetPosition)
        {
            throw new System.NotImplementedException();
        }

        public override void OnAnimEvent(string phase)
        {
            if (phase == "Land")
            {
                Collider[] hits = Physics.OverlapSphere(enemy.transform.position, config.attackRange, LayerMask.GetMask("Player"));
                foreach (var hit in hits)
                {
                    hit.GetComponent<PlayerHealth>()?.TakeDamage(config.attackDamage);
                    // TODO: VFX over here
                }
            }
        }
    }
}
