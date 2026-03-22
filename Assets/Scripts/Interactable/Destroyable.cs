using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.InteractionSystem
{
    public class Destroyable : MonoBehaviour, FPS.IDamagable
    {
        [SerializeField] private float maxHealth;
        public UnityEvent OnTakeDamage;
        public UnityEvent OnDeath;
        private float health;

        void Awake()
        {
            health = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (health <= 0) return;
            health -= damage;
            OnTakeDamage?.Invoke();
            if (health <= 0)
            {
                health = 0.0f;
                OnDeath?.Invoke();
            }
        }

        public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitNormal, Vector3 hitDirection)
        {
            TakeDamage(damage);
        }
    }
}
