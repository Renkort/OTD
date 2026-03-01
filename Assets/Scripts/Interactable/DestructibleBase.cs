using Akkerman.FPS;
using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.InteractionSystem
{
    
    [RequireComponent(typeof(Collider))]
    public abstract class DestructibleBase : MonoBehaviour, IDamagable
    {
        [Header("Health")]
        [SerializeField] protected float maxHealth = 100f;
        protected float currentHealth;

        [Header("Destruction")]
        [SerializeField] private GameObject fracturedPrefab;
        [SerializeField] private ParticleSystem breakParticles;
        [SerializeField] private AudioClip breakSound;

        [Header("Events")]
        public UnityEvent<Vector3, Vector3> OnDestroyed; // hitPoint + hitDirection

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            
            foreach (Rigidbody rb in fracturedPrefab.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }
        }

        public virtual void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                DestroyObject(hitPoint, hitNormal, hitDirection);
            }
            else
            {
                OnHitEffect(hitPoint);
            }
        }

        protected virtual void DestroyObject(Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection)
        {
            if (fracturedPrefab != null)
            {
                GameObject fractured = Instantiate(fracturedPrefab, transform.position, transform.rotation);
                // Apply impuls for every part
                foreach (var rb in fractured.GetComponentsInChildren<Rigidbody>())
                {
                    rb.isKinematic = false;
                    rb.AddForceAtPosition(hitDirection * 15f + Random.onUnitSphere * 5f, hitPoint, ForceMode.Impulse);
                }
            }

            // VFX + SFX
            if (breakParticles) Instantiate(breakParticles, hitPoint, Quaternion.LookRotation(hitNormal));
            if (breakSound) AudioSource.PlayClipAtPoint(breakSound, hitPoint);

            OnDestroyed?.Invoke(hitPoint, hitDirection);

            Destroy(gameObject); // or ObjectPooler.ReturnToPool
        }

        protected virtual void OnHitEffect(Vector3 point) { /* переопределяется в наследниках */ }
    }
}