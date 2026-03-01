using Akkerman.SaveSystem;
using Akkerman.UI;
using UnityEngine;

namespace Akkerman.FPS
{
    
    public class PlayerHealth : MonoBehaviour, IDamagable, IDataPersistance
    {
        [SerializeField] private float maxHealth = 100.0f;
        private float currentHealth;

        void Awake()
        {
            // load player health from last save IData...
            currentHealth = maxHealth;
        }
        void Start()
        {
            GameUI.Instance.IngameUI.SetHealthUI(currentHealth);
        }

        public void LoadData(GameData data)
        {
            currentHealth = data.PlayerCurrentHealth;

            GameUI.Instance.IngameUI.SetHealthUI(currentHealth);
        }

        public void SaveData(ref GameData data)
        {
            data.PlayerCurrentHealth = currentHealth;
        }

        public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitNormal, Vector3 hitDirection)
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
