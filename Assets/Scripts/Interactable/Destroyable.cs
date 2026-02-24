using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.InteractionSystem
{
    public class Destroyable : MonoBehaviour, FPS.IDamagable
    {
        public UnityEvent OnTakeDamage;
        public void TakeDamage(int damage)
        {
            OnTakeDamage?.Invoke();            
        }
    }
}
