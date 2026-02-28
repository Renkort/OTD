using Akkerman.UI;
using UnityEngine;

namespace Akkerman.FPS
{
    
    public class PlayerHealth : MonoBehaviour, IDamagable
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;

        void Awake()
        {
            // load player health from last save IData...
            currentHealth = maxHealth;
        }
        void Start()
        {
            GameUI.Instance.IngameUI.SetHealthUI(currentHealth);
        }

        public void TakeDamage(int damage)
        {
            if (Player.Instance.IsDead) return;
            
            currentHealth -= damage;
            if (currentHealth <= 0 )
            {
                currentHealth = 0;
                Player.Instance.Kill();
            }
            
            GameUI.Instance.IngameUI.SetHealthUI(currentHealth);
        }
    }
}
