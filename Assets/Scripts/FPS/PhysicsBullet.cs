using UnityEngine;


namespace Akkerman.FPS
{
    
    public class PhysicsBullet : Bullet
    {
        [Header("PHYSICS SETTINGS")]
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private int maxBounces = 0;

        private Vector3 velocity;
        private int bounceCount = 0;

        protected override void Start()
        {
            base.Start();
            velocity = transform.forward * moveSpeed;
        }

        protected override void Move()
        {
            velocity += Vector3.down * gravity * Time.deltaTime;

            transform.position += velocity * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(velocity.normalized);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            // base.OnTriggerEnter(other);

            if (((1 << other.gameObject.layer) & collisionHitMask) != 0)
            {
                if (bounceCount < maxBounces)
                {
                    Bounce(other);
                    bounceCount++;
                }
                else
                {
                    base.OnHit(other);
                }
            }
        }

        private void Bounce(Collider other)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position - velocity.normalized * 0.1f, velocity.normalized, out hit, 0.2f, collisionHitMask))
            {
                velocity = Vector3.Reflect(velocity, hit.normal);
                transform.rotation = Quaternion.LookRotation(velocity.normalized);
            }
        }
    }
}
