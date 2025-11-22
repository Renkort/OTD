using UnityEngine;


namespace Akkerman.FPS.Usables
{
    
    public class SpringFlashlight : MonoBehaviour
    {
        [Header("References")]
        public Transform playerCamera;

        [Header("Spring Settings")]
        public float springStrength = 100f;
        public float springDamping = 10f;
        public Vector3 positionOffset = new Vector3(0.3f, -0.2f, 0.1f);

        private Vector3 rotationVelocity;
        private Vector3 positionVelocity;

        void Update()
        {
            if (playerCamera == null) return;

            SpringFollow();
        }

        void SpringFollow()
        {
            Vector3 targetPosition = playerCamera.TransformPoint(positionOffset);
            Vector3 positionError = targetPosition - transform.position;

            Vector3 acceleration = positionError * springStrength - positionVelocity * springDamping;
            positionVelocity += acceleration * Time.deltaTime;
            transform.position += positionVelocity * Time.deltaTime;

            Quaternion targetRotation = playerCamera.rotation;
            Quaternion rotationError = targetRotation * Quaternion.Inverse(transform.rotation);
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);

            if (angle > 180f) angle -= 360f;

            Vector3 rotationAcceleration = axis * (angle * springStrength * 0.01f) - rotationVelocity * springDamping;
            rotationVelocity += rotationAcceleration * Time.deltaTime;

            Vector3 rotationStep = rotationVelocity * Time.deltaTime;
            transform.rotation *= Quaternion.Euler(rotationStep);
        }
    }
}