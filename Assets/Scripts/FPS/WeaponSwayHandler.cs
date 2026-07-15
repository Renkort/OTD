using UnityEngine;

namespace Akkerman.FPS
{
    // (Same object with weaponSlots in WeaponInventory).
    public class WeaponSwayHandler : MonoBehaviour
    {
        [SerializeField] private WeaponInventory inventory;
        [SerializeField] private WeaponSwaySettings defaultSettings; // while weapon isn't selected

        private WeaponSwaySettings settings;

        private Vector3 initialLocalPos;
        private Quaternion initialLocalRot;

        private float speed01;   // 0 = idle, 1 = walk, >1 = run
        private bool isRunning;
        private bool isGrounded = true;

        private float bobTimer;

        private void Awake()
        {
            initialLocalPos = transform.localPosition;
            initialLocalRot = transform.localRotation;
            settings = defaultSettings;
        }

        private void OnEnable()
        {
            if (inventory != null)
                inventory.OnWeaponSwitched += HandleWeaponSwitched;
        }

        private void OnDisable()
        {
            if (inventory != null)
                inventory.OnWeaponSwitched -= HandleWeaponSwitched;
        }

        private void HandleWeaponSwitched(HoldableItemData data, int slotIndex)
        {
            settings = data.swaySettings;
        }

        // Вызывается каждый кадр из твоего PlayerMovement/CharacterController
        public void SetMovementState(float normalizedSpeed, bool running, bool grounded)
        {
            speed01 = normalizedSpeed;
            isRunning = running;
            isGrounded = grounded;
        }

        private void LateUpdate()
        {
            Vector3 lookSway = CalculateLookSway();
            Vector3 idleOffset = CalculateIdleSway();
            Vector3 bobOffset = CalculateBob();

            Vector3 targetPos = initialLocalPos + idleOffset + bobOffset;
            Quaternion targetRot = initialLocalRot * Quaternion.Euler(lookSway.y, -lookSway.x, -lookSway.x * 0.5f);

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * settings.bobSmooth);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * settings.swaySmooth);
        }

        private Vector3 CalculateLookSway()
        {
            float mouseX = Input.GetAxis("Mouse X") * settings.swayAmount;
            float mouseY = Input.GetAxis("Mouse Y") * settings.swayAmount;

            mouseX = Mathf.Clamp(mouseX, -settings.maxSwayAngle, settings.maxSwayAngle);
            mouseY = Mathf.Clamp(mouseY, -settings.maxSwayAngle, settings.maxSwayAngle);

            return new Vector3(mouseX, mouseY, 0f);
        }

        private Vector3 CalculateIdleSway()
        {
            if (speed01 > 0.05f) return Vector3.zero; // идл-покачивание только когда стоим

            float sin = Mathf.Sin(Time.time * settings.idleSwaySpeed) * settings.idleSwayAmount;
            float cos = Mathf.Cos(Time.time * settings.idleSwaySpeed * 0.5f) * settings.idleSwayAmount;

            return new Vector3(cos, sin, 0f);
        }

        private Vector3 CalculateBob()
        {
            if (speed01 <= 0.05f || !isGrounded)
            {
                bobTimer = 0f;
                return Vector3.zero;
            }

            float bobSpeed = isRunning ? settings.runBobSpeed : settings.walkBobSpeed;
            float bobAmount = isRunning ? settings.runBobAmount : settings.walkBobAmount;

            bobTimer += Time.deltaTime * bobSpeed * speed01;

            float bobX = Mathf.Cos(bobTimer) * bobAmount * 0.5f;
            float bobY = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmount; // "восьмёрка" — классический bob-паттерн

            return new Vector3(bobX, bobY, 0f);
        }
    }
}
