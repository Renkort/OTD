using UnityEngine;


namespace Akkerman.FPS
{
    public class HitscanBullet : Bullet
    {
        [Header("HITSCAN SETTINGS")]
        [SerializeField] private float maxDistance = 1000f;
        [SerializeField] private LineRenderer trailRenderer;
        [SerializeField] private float trailLifetime = 0.05f;

        private bool hasHit = false;
        private Vector3 hitPoint;
        private float trailTimer;

        protected override void Start()
        {
            base.Start();

            trailTimer = trailLifetime;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, collisionHitMask))
            {
                hasHit = true;
                hitPoint = hit.point;
                OnHit(hit.collider);
            }
            else
            {
                hitPoint = transform.position + transform.forward * maxDistance;
            }

            if (trailRenderer != null)
            {
                trailRenderer.SetPosition(0, transform.position);
                trailRenderer.SetPosition(1, hitPoint);
            }
        }

        protected override void Update()
        {
            base.Update();
            
            trailTimer -= Time.deltaTime;
            trailRenderer.widthCurve.keys[0].value = trailTimer;
            if (trailTimer <= 0f)
                trailRenderer.enabled = false;
        }

        protected override void Move()
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            if (hasHit && Vector3.Distance(transform.position, hitPoint) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
        
    }
}
