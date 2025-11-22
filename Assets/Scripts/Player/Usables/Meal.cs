using System.Collections;
using Akkerman.UI;
using UnityEngine;

namespace Akkerman.FPS.Usables
{
    public class Meal : HoldableItem
    {
        [Header("References")]
        public Transform playerCamera;
        [Header("PARAMETERS")]
        [SerializeField] private int amount = 1;
        [Header("Smooth Settings")]
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float positionSpeed = 5f;
        [SerializeField] private Vector3 positionOffset = new Vector3(-0.1f, -0.15f, 0.2f);
        [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 0f, 0f);
        [SerializeField] private Vector3 eatPositionOffset;
        [SerializeField] private Vector3 eatRotationOffset;
        [SerializeField] private float eatTime = 2f;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip eatSFX;

        private Vector3 rotationVelocity;
        private Vector3 positionVelocity;
        private Vector3 targetPosition;
        private bool isUsing = false;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (playerCamera == null) return;

            FollowCamera();
            if (Input.GetMouseButtonDown(0) && !isUsing && amount > 0)
            {
                StartCoroutine(Use());
            }
        }

        private void FollowCamera()
        {
            targetPosition = playerCamera.TransformPoint(positionOffset);
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref positionVelocity,
                positionSpeed * Time.deltaTime
            );
            // Vector3 targetRotation = playerCamera.rotation.eulerAngles - rotationOffset;
            //Quaternion targetRotation = playerCamera.rotation * Quaternion.Euler(rotationOffset);
            Quaternion targetRotation = playerCamera.rotation;
            Vector3 targetEuler = targetRotation.eulerAngles + rotationOffset;
            targetRotation = Quaternion.Euler(targetEuler);

            transform.rotation = QuaternionUtil.SmoothDamp(
                transform.rotation,
                targetRotation,
                ref rotationVelocity,
                rotationSpeed * Time.deltaTime
            );
        }

        public override void UpdateUI()
        {
            if (amount < 0)
                amount = 0;
            GameUI.Instance.IngameUI.SetAmmoUI($"{amount}", null);
        }

        private IEnumerator Use()
        {
            isUsing = true;
            Vector3 idlePositionOffset = positionOffset;
            Vector3 idleRotationOffset = rotationOffset;
            positionOffset = eatPositionOffset;
            rotationOffset = eatRotationOffset;
            if (eatSFX != null)
                audioSource.PlayOneShot(eatSFX);

            yield return new WaitForSeconds(eatTime);
            amount--;
            UpdateUI();
            positionOffset = idlePositionOffset;
            rotationOffset = idleRotationOffset;
            isUsing = false;
        }
    }
}
