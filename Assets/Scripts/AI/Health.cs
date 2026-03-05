using System;
using UnityEngine;

namespace Akkerman.AI
{
    public class Health : MonoBehaviour
    {
        public event Action OnDeath;
        private float currentHealth;
        public bool IsDead => currentHealth <= 0;
        public void Initialize(float maxHealth) => currentHealth = maxHealth;
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                OnDeath?.Invoke();
        }
    }
    
}
