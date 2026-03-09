using UnityEngine;


namespace Akkerman.AI
{
    public abstract class BaseCombat : MonoBehaviour, ICombat
    {
        protected EnemyConfig config;
        protected Enemy enemy;
        protected float lastAttackTime;
        protected float cooldownTimer;

        public virtual void Initialize(EnemyConfig config, Enemy e)
        {
            this.config = config;
            enemy = e;
        }
        public virtual bool CanAttack() => Time.time > lastAttackTime + config.attackCooldown;
        public abstract void Attack(Vector3 targetPosition);
        public abstract void AttackUpdate(Vector3 targetPosition);
        public abstract void OnAnimEvent(string phase);
        public float GetRange() => config.attackRange;

    }

}
