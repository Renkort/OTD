using UnityEngine;


namespace Akkerman.InteractionSystem
{
    
    public class GlassDestructible : DestructibleBase
    {
        public Material crackMaterial; // shader with трещинами
        private MeshRenderer meshRenderer;

        protected override void Awake()
        {
            base.Awake();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        protected override void OnHitEffect(Vector3 point)
        {
            // Показываем трещины (можно через decal или material property)
            if (meshRenderer)
            {
                meshRenderer.material.SetVector("_CrackPosition", point);
                meshRenderer.material.SetFloat("_CrackAmount", 1f - currentHealth / maxHealth);
            }
        }

        protected override void DestroyObject(Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection)
        {
            // Дополнительно: разбиваем на мелкие осколки через particles + pre-fractured mesh
            base.DestroyObject(hitPoint, hitNormal, hitDirection);
        }
    }
}