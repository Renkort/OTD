using UnityEngine;


namespace Akkerman.Graphics
{
    
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraDitherEffect : MonoBehaviour
    {
        [Header("Dithering Settings")]
        public Material ditherMaterial;
        [Range(0f, 1f)] public float ditherStrength = 1f;
        [Range(2, 64)] public float colorSteps = 8;

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (ditherMaterial != null)
            {
                ditherMaterial.SetFloat("_DitherStrength", ditherStrength);
                ditherMaterial.SetFloat("_ColorSteps", colorSteps);
                UnityEngine.Graphics.Blit(src, dest, ditherMaterial);
            }
            else
            {
                UnityEngine.Graphics.Blit(src, dest);
            }
        }
    }
}