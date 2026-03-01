using UnityEngine;
using Akkerman.UI;

namespace Akkerman.FPS.Usables
{
    
    public class Throwable : HoldableItem
    {

        [SerializeField] private bool isUsing = false;
        [SerializeField] private GameObject throwablePrefab;

        [SerializeField] private int damage;
        [SerializeField] private float delay = 3f;
        [SerializeField] private float damageRadius = 6f;
        [SerializeField] private float explosionForce = 1600f;
        [SerializeField] private GameObject explodeEffect;
        [SerializeField] private float throwForce = 10f;
        [SerializeField] private LayerMask damageableLayer;
        private float forceMultiplier = 0f;
        private float forceMultiplierLimit = 5f;
        public float ForceModifierLimit => forceMultiplierLimit;
        [SerializeField] private Transform throwableSpawn;

        [SerializeField] private int amount = 0;

        private float countdown;
        private bool hasExploded = false;
        private bool hasBeenThrown = false;

        public enum ThrowableType
        {
            Grenade
        }

        public ThrowableType throwableType;

        private void Start()
        {
            countdown = delay;
            if (isUsing)
                UpdateUI();
        }

        private void Update()
        {
            if (hasBeenThrown)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0 && !hasExploded)
                {
                    hasExploded = true;
                    Explode();
                }
            }
            if (!isUsing)
                return;
            else if (Player.Instance.DialogueUI.IsOpen)
                return;
            if (Input.GetMouseButton(0) && forceMultiplier < forceMultiplierLimit && amount > 0)
            {
                forceMultiplier += Time.deltaTime * throwForce;
                GameUI.Instance.IngameUI.forceModifierSlider.gameObject.SetActive(true);
                GameUI.Instance.IngameUI.DisplayForceModifierSlider(forceMultiplier);
            }
            if (amount > 0 && Input.GetMouseButtonUp(0))
            {
                ThrowLethal();
            }
        }

        private void ThrowLethal()
        {
            amount--;
            GameUI.Instance.IngameUI.forceModifierSlider.gameObject.SetActive(false);
            UpdateUI();
            GameObject throwable = Instantiate(throwablePrefab, throwableSpawn.position, Camera.main.transform.rotation);
            Rigidbody rb = throwable.GetComponent<Rigidbody>();
            rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

            forceMultiplier = 0f;
            throwable.GetComponent<Throwable>().hasBeenThrown = true;
        }

        private void Explode()
        {
            GetThrowableEffect();
            Destroy(gameObject);
        }

        private void GetThrowableEffect()
        {
            switch (throwableType)
            {
                case ThrowableType.Grenade:
                    ExplodeEffect();
                    break;
            }
        }

        private void ExplodeEffect()
        {
            GameObject explosionEffect = Instantiate(explodeEffect, transform.position, explodeEffect.transform.rotation);
            Player.Instance.FpsController.ShakeCamera(0.1f, 0.1f);
            
            Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius, damageableLayer);
            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.attachedRigidbody; // faster than GetComponent
                Vector3 hitDirection = (collider.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                float forceFalloff =  Mathf.Pow(1f - distance / damageRadius, 2f); // квадратичный спад урона; // [0, 1] 

                if (rb != null)
                {
                    Debug.Log($"Detected object name: {collider.gameObject.name}");
                    //rb.AddExplosionForce(explosionForce, transform.position, damageRadius, 1f);
                    rb.AddForce(hitDirection * explosionForce * forceFalloff, ForceMode.Impulse);
                }
                if (collider is CharacterController)
                    continue;
                float finalDamage = damage * forceFalloff;

                IDamagable parentDamagable = collider.GetComponentInParent<IDamagable>();
                Vector3 hitPoint = collider.ClosestPoint(transform.position);
                Vector3 hitNormal = -hitDirection; // from center to outer
                if (collider.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    Debug.Log($"DEGUG: EXPLOSION DISTANCE TO OBJECT: {collider.gameObject.name}:{distance}");
                    damagable.TakeDamage(finalDamage, hitPoint, hitNormal, hitDirection);
                    continue;
                }
                else if (parentDamagable != null)
                {
                    Debug.Log($"DEGUG: EXPLOSION DISTANCE TO OBJECT: {collider.gameObject.name}:{distance}");
                    Debug.Log($"DEGUG: EXPLOSION COMPONENT IN PARENT");
                    parentDamagable.TakeDamage(finalDamage, hitPoint, hitNormal, hitDirection);
                }
            }
            Destroy(explosionEffect, explosionEffect.GetComponent<ParticleSystem>().main.duration);
            Destroy(gameObject);
        }
        public override void UpdateUI()
        {
            GameUI.Instance.IngameUI.SetAmmoUI(amount.ToString(), null);
        }

        public void AddAmmo(int amount)
        {
            this.amount += amount;
            UpdateUI();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, damageRadius);
        }
    }
}
