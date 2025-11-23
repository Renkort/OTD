using System.Collections.Generic;
using UnityEngine;


namespace Akkerman.FPS
{
    
    public abstract class Bullet : MonoBehaviour
    {
        [Header("BASIC SETTINGS")]
        [SerializeField] protected int damage;
        [SerializeField] protected float moveSpeed = 100f;
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] protected LayerMask collisionHitMask;
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

        protected virtual void OnHit(Collider other)
        {
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(damage);
            }

            // spawn hit VFX (instantiate bullet impact)
            // bool hasEffect = false;
            // foreach (var effect in impactEffects)
            // {
            //     if (other.gameObject.CompareTag(effect.SurfaceTag))
            //     {
            //         CreateBulletImpactEffect(collision, effect.ImpactEffect);
            //         hasEffect = true;
            //         break;
            //     }
            // }
            // if (!hasEffect) // create default hit effect
            //     CreateBulletImpactEffect(collision, impactEffects[0].ImpactEffect);
            // if (other.gameObject.CompareTag("Enemy"))
            // {
            //     // collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            // }

            Destroy(gameObject);
        }
        // protected virtual void OnCollisionEnter(Collision collision)
        // {
        //     if ( ((1 << collision.gameObject.layer) & collisionHitMask) != 0)
        //     {
        //         OnHit(collision.collider);
        //     }
        // }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if ( ((1 << other.gameObject.layer) & collisionHitMask) != 0)
            {
                OnHit(other);
            }
        }

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
