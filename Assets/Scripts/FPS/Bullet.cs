using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Akkerman.FPS
{
    
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private List<ImpactSurfaceType> impactEffects;
        private void OnCollisionEnter(Collision collision)
        {
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
            if (!hasEffect)
                CreateBulletImpactEffect(collision, impactEffects[0].ImpactEffect);
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        void CreateBulletImpactEffect(Collision collision, GameObject impactEffect)
        {
            ContactPoint contact = collision.contacts[0];

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
}
