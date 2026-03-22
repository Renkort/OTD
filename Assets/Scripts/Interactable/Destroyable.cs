using System;
using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.InteractionSystem
{
    public class Destroyable : MonoBehaviour, FPS.IDamagable
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private bool usePhysics;
        [SerializeField] private Rigidbody rb;
        public UnityEvent OnTakeDamage;
        public UnityEvent OnDeath;
        public Action<float> onTakeDamage;
        private float health;
        private float damageForceImpact = 100f;
        float forceFalloff;
        public float MaxHealth => maxHealth; 

        void Awake()
        {
            health = maxHealth;
            forceFalloff = Mathf.Pow(1f - rb.mass / damageForceImpact, 2f);
        }

        public void TakeDamage(float damage)
        {
            if (health <= 0) return;
            health -= damage;
            OnTakeDamage?.Invoke();
            onTakeDamage?.Invoke(damage);
            if (health <= 0)
            {
                health = 0.0f;
                OnDeath?.Invoke();
            }
        }

        public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitNormal, Vector3 hitDirection)
        {
            TakeDamage(damage);
            if (usePhysics)
            {
                rb.AddForce(damageForceImpact * forceFalloff * hitDirection, ForceMode.Impulse);
            }
        }
    }
}
