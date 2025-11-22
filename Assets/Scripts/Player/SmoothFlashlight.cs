using UnityEngine;
using Akkerman.SaveSystem;

namespace Akkerman.FPS.Usables
{
    
    public class SmoothFlashlight : MonoBehaviour, IDataPersistance
    {
        [Header("References")]
        public Transform playerCamera;

        [Header("Smooth Settings")]
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float positionSpeed = 5f;
        [SerializeField] private Vector3 positionOffset = new Vector3(-0.1f, -0.15f, 0.2f);
        [SerializeField] private Light flashlightLight;
        [SerializeField] private AudioClip toggleSound;

        [SerializeField] private AudioSource audioSource;

        private Vector3 rotationVelocity;
        private Vector3 positionVelocity;
        private KeyCode toggleKey = KeyCode.F;

        void Start()
        {
            if (flashlightLight == null)
                flashlightLight = GetComponentInChildren<Light>();

            audioSource = GetComponent<AudioSource>();
        }
        void Update()
        {
            if (playerCamera == null) return;

            if (Input.GetKeyDown(toggleKey))
            {
                ToggleFlashlight();
            }

            FollowCamera();
        }

        void ToggleFlashlight()
        {
            if (flashlightLight != null)
            {
                flashlightLight.enabled = !flashlightLight.enabled;

                if (audioSource != null && toggleSound != null)
                    audioSource.PlayOneShot(toggleSound);
            }
        }


        void FollowCamera()
        {
            Vector3 targetPosition = playerCamera.TransformPoint(positionOffset);
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref positionVelocity,
                positionSpeed * Time.deltaTime
            );

            transform.rotation = QuaternionUtil.SmoothDamp(
                transform.rotation,
                playerCamera.rotation,
                ref rotationVelocity,
                rotationSpeed * Time.deltaTime
            );
        }

        public void SaveData(ref GameData data)
        {
            Debug.Log($"Is Flashlight enabled: {flashlightLight.enabled}");
            data.IsFlashlightActive = flashlightLight.enabled;
        }

        public void LoadData(GameData gameData)
        {
            flashlightLight.enabled = gameData.IsFlashlightActive;
        }
    }
    public static class QuaternionUtil
    {
        public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Vector3 velocity, float smoothTime)
        {
            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
                Mathf.SmoothDampAngle(c.x, t.x, ref velocity.x, smoothTime),
                Mathf.SmoothDampAngle(c.y, t.y, ref velocity.y, smoothTime),
                Mathf.SmoothDampAngle(c.z, t.z, ref velocity.z, smoothTime)
            );
        }
    }
}