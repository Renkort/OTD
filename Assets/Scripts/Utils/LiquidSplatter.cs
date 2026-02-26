using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.Utils
{
    
    public class LiquidSplatter : MonoBehaviour
    {
        [Header("Decal Settings")]
        public GameObject decalPrefab;
        public int maxDecals = 150;
        public float decalLifetime = 90f;
        public float spawnChance = 0.5f;
        public float minVelocity = 1.5f; // only fast drops make stains

        [SerializeField] private ParticleSystem liquidVFX;
        [SerializeField] private LayerMask colideLayer;
        private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        private Queue<GameObject> activeDecals = new Queue<GameObject>();

        void Start()
        {
            collisionEvents = new List<ParticleCollisionEvent>(128);
        }

        void OnParticleCollision(GameObject other)
        {
            // if (other.layer != LayerMask.NameToLayer("Ground") && // wtf?!
            //     other.layer != LayerMask.NameToLayer("Walls")) return;
            if (1 << other.layer != colideLayer)
                return;
            Debug.Log($"DEBUG: PARTICLE HIT");

            int eventsCount = liquidVFX.GetCollisionEvents(other, collisionEvents);

            for (int i = 0; i < eventsCount; i++)
            {
                ParticleCollisionEvent hit = collisionEvents[i];

                // filters
                if (Random.value > spawnChance) continue;
                if (hit.velocity.magnitude < minVelocity) continue;

                if (activeDecals.Count >= maxDecals)
                {
                    GameObject old = activeDecals.Dequeue();
                    Destroy(old);
                }

                GameObject decal = Instantiate(decalPrefab, hit.intersection + hit.normal * 0.00001f, Quaternion.identity);

                // decal.transform.rotation = Quaternion.LookRotation(-hit.normal);

                // decal.transform.Rotate(Vector3.forward, Random.Range(0f, 360f), Space.Self);
                float scaleVar = Random.Range(0.02f, 0.06f);
                decal.transform.localScale = new Vector3(scaleVar, 0.001f, scaleVar);

                decal.transform.SetParent(other.transform);

                Destroy(decal, decalLifetime);

                activeDecals.Enqueue(decal);
            }
        }
    }
}
