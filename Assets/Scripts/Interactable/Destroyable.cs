using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.InteractionSystem
{
    public class Destroyable : MonoBehaviour, FPS.IDamagable
    {
        public UnityEvent OnTakeDamage;

        public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitNormal, Vector3 hitDirection)
        {
            OnTakeDamage?.Invoke();
        }
    }
}
