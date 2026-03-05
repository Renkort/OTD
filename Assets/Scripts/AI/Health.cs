using System;
using Akkerman.FPS;
using UnityEngine;

namespace Akkerman.AI
{
    public class Health : MonoBehaviour, IDamagable
    {
        public event Action OnDeath;
        private float currentHealth;
        public bool IsDead => currentHealth <= 0;
        float damageForceImpact = 100f;
        Rigidbody rb;

        public void Initialize(float maxHealth)
        {
            currentHealth = maxHealth;
            rb = GetComponent<Rigidbody>();
        }

        public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitNormal, Vector3 hitDirection)
        {
            currentHealth -= damage;
            if (rb) // impact
            {
                float forceFalloff = Mathf.Pow(1f - rb.mass / damageForceImpact, 2f);
                rb.AddForce(hitDirection * damageForceImpact * forceFalloff, ForceMode.Impulse);
            }

            if (currentHealth <= 0)
                OnDeath?.Invoke();
        }

        public void TakeDamage(float damage)
        {
            throw new NotImplementedException();
        }
    }
    
}
