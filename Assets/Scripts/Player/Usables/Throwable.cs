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
            GameObject explosionEffect = Instantiate(explodeEffect, transform.position, explodeEffect.transform.rotation);// change rotation
            Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
            Player.Instance.FpsController.ShakeCamera(0.1f, 0.1f);
            foreach (Collider objectInRange in colliders)
            {
                Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log($"Detected object name: {objectInRange.gameObject.name}");
                    //rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
                    Vector3 direction = -(transform.position - objectInRange.gameObject.transform.position);
                    direction.Normalize();
                    rb.AddForce(direction * explosionForce, ForceMode.Impulse);
                }
                // enemy takes damage over here
                IDamagable parentDamagable = objectInRange.GetComponentInParent<IDamagable>();
                if (objectInRange.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    float distance = Vector3.Distance(objectInRange.transform.position, transform.position);
                    Debug.Log($"DEGUG: EXPLOSION DISTANCE TO OBJECT: {objectInRange.gameObject.name}:{distance}");
                    int finalDamage = damage - (int)distance;
                    damagable.TakeDamage(finalDamage);
                }
                else if (parentDamagable != null)
                {
                    float distance = Vector3.Distance(objectInRange.transform.position, transform.position);
                    int finalDamage = damage - (int)distance;
                    Debug.Log($"DEGUG: EXPLOSION DISTANCE TO OBJECT: {objectInRange.gameObject.name}:{distance}");
                    parentDamagable.TakeDamage(finalDamage);
                }
            }
            Destroy(explosionEffect, explosionEffect.GetComponent<ParticleSystem>().main.duration);
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
