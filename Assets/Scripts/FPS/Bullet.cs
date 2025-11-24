using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;


namespace Akkerman.FPS
{
    
    public abstract class Bullet : MonoBehaviour
    {
        [Header("BASIC SETTINGS")]
        [SerializeField] protected int damage;
        [SerializeField] protected float moveSpeed = 100f;
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private List<ImpactSurfaceType> impactEffects;

        protected float timer;
        protected Vector3 startPosition;

        protected virtual void Start()
        {
            startPosition = transform.position;
            timer = lifeTime;
        }

        protected virtual void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Destroy(gameObject);
            }

            Move();
        } 

        protected abstract void Move();

        protected virtual void OnHit(Collider other, Vector3 hitPosition, Vector3 hitNormal)
        {
            IDamagable damagable = other.GetComponentInParent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(damage);
            }
            bool hasEffect = false;
            foreach (var effect in impactEffects)
            {
                if (other.gameObject.CompareTag(effect.SurfaceTag))
                {
                    CreateBulletImpactEffect(hitPosition, hitNormal, effect.ImpactEffect);
                    hasEffect = true;
                    break;
                }
            }
            if (!hasEffect) // create default hit effect
                CreateBulletImpactEffect(hitPosition, hitNormal, impactEffects[0].ImpactEffect);

            Destroy(gameObject, 0.1f);
        }
        protected virtual void OnHit(Collision collision)
        {
            IDamagable damagable = collision.collider.GetComponentInParent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(damage);
            }

            bool hasEffect = false;
            foreach (var effect in impactEffects)
            {
                if (collision.gameObject.CompareTag(effect.SurfaceTag))
                {
                    CreateBulletImpactEffect(collision, effect.ImpactEffect);
                    hasEffect = true;
                    break;
                }
            }
            if (!hasEffect) // create default hit effect
                CreateBulletImpactEffect(collision, impactEffects[0].ImpactEffect);

            Destroy(gameObject);
        }
        // protected virtual void OnCollisionEnter(Collision collision)
        // {
        //     if ( ((1 << collision.gameObject.layer) & collisionHitMask) != 0)
        //     {
        //         OnHit(collision.collider);
        //     }
        // }

        protected virtual void CreateBulletImpactEffect(Collision collision, GameObject impactEffect)
        {
            // ContactPoint contact = collision.contacts[0];
            ContactPoint contact = collision.GetContact(0);

            GameObject hole = Instantiate(
                impactEffect,
                contact.point,
                Quaternion.LookRotation(contact.normal)
            );

            hole.transform.SetParent(collision.gameObject.transform);
        }
        protected virtual void CreateBulletImpactEffect(Vector3 hitPoint, Vector3 hitNormal, GameObject impactEffect)
        {
            // ContactPoint contact = collision.contacts[0];

            GameObject hole = Instantiate(
                impactEffect,
                hitPoint,
                Quaternion.LookRotation(hitNormal)
            );
        }
    }

    [System.Serializable]
    public struct ImpactSurfaceType
    {
        public string SurfaceTag;
        public GameObject ImpactEffect;
    }

    public interface IDamagable
    {
        void TakeDamage(int damage);
    }
}
