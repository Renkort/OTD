using Akkerman.FPS;
using UnityEngine;


namespace Akkerman.AI
{
    public class MeleeAttack : BaseCombat
    {
        public override void Attack(Vector3 targetPosition)
        {
            lastAttackTime = Time.time;
        }

        public override void OnAnimEvent(string phase)
        {
            if (phase == "Hit")
            {
                Collider[] hits = Physics.OverlapSphere(enemy.transform.position + enemy.transform.forward * 1.5f, 1.5f, LayerMask.GetMask("Player"));
                foreach (var hit in hits)
                {
                    hit.GetComponent<PlayerHealth>()?.TakeDamage(config.attackDamage);
                }
            }
        }
    }
}
