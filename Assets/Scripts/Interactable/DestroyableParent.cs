using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Akkerman.InteractionSystem
{
    public class DestroyableParent : MonoBehaviour
    {
        public float healthSum = 0.0f;
        [SerializeField] private List<Destroyable> children;
        public UnityEvent OnDeath;

        void Start()
        {
            for (int i = 0; i < children.Count; i++)
            {
                healthSum += children[i].MaxHealth;
                children[i].onTakeDamage += TakeDamage;
            }
        }

        public void TakeDamage(float damage)
        {
            if (healthSum <= 0) return;
            healthSum -= damage;
            if (healthSum <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}
