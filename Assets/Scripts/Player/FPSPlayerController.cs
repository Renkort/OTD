using System.Collections;
using UnityEngine;
using Akkerman.SaveSystem;

namespace Akkerman.FPS
{
    
    [RequireComponent(typeof(CharacterController), typeof(AudioSource))]
    public class FPSPlayerController : MonoBehaviour, IDataPersistance
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float crouchSpeed = 2.0f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float gravity = -15f;
        [SerializeField] private float crouchHeight = 0.5f;
        [SerializeField] private float standHeight = 1.25f;
        [SerializeField] private float crouchTransitionSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 15f;
        [SerializeField] private float airControl = 0.2f;

        [Header("Camera")]
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float tiltAmount = 0.2f;
        [SerializeField] private float tiltSpeed = 5f;

        [Header("Head Bobbing")]
        [SerializeField] private float walkBobSpeed = 14f;
        [SerializeField] private float walkBobAmount = 0.05f;
        [SerializeField] private float runBobSpeed = 18f;
        [SerializeField] private float runBobAmount = 0.1f;
        [SerializeField] private float landBobAmount = 0.02f;
        private float defaultCameraY;
        private float headBobTimer;

        [Header("Audio")]
        [SerializeField] private SurfaceFootsteps[] footstepSounds;
        [SerializeField] private float footstepInterval = 0.5f;
        [SerializeField] private Transform footstepOrigin;
        [SerializeField] private float raycastDistance = 0.2f;
        [SerializeField] private LayerMask groundLayer;
        private float footstepTimer;
        private AudioSource audioSource;

        private CharacterController characterController;
        private Vector3 moveDirection;
        private Vector3 currentVelocity;
        private float xRotation, yRotation;
        private float currentSpeed;
        private bool isCrouching;
        private bool isGrounded;
        private bool wasGrounded;
        private float currentHeight;
        private float originalStepOffset;
        private bool canMove = true, canLookAround = true,
        usePhysics = true;
        private float tilt;
        private bool isDead = false;
        private bool isLoading = false;
        float xShakeRotation, yShakeRotation;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            defaultCameraY = playerCamera.localPosition.y;
            currentHeight = standHeight;
            originalStepOffset = characterController.stepOffset;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            yRotation = gameObject.transform.localRotation.eulerAngles.y;

            if (PlayerPrefs.HasKey("masterSensitivity"))
                mouseSensitivity = 25 * PlayerPrefs.GetFloat("masterSensitivity");
        }

        private void Update()
        {
            if (isLoading)
                return;
            if (isDead)
            {
                HandleBodyPhysics();
                ApplyFinalMovements();
                return;
            }

            HandleGroundCheck();
            HandleMovement();
            HandleMouseLook();
            HandleJump();
            HandleCrouch();
            HandleHeadBob();
            HandleFootsteps();
            ApplyFinalMovements();
        }

        private void HandleGroundCheck()
        {
            wasGrounded = isGrounded;
            isGrounded = characterController.isGrounded;

            if (isGrounded && moveDirection.y < 0)
            {
                moveDirection.y = -2f;
                characterController.stepOffset = originalStepOffset;

                if (!wasGrounded && moveDirection.y < 5f)
                {
                    ShakeCamera(0.1f, landBobAmount);
                    PlayFootstepSound();
                }
            }
            else if (!isGrounded)
            {
                characterController.stepOffset = 1;
            }
        }

        private void HandleMovement()
        {
            isGrounded = characterController.isGrounded;

            if (isGrounded && moveDirection.y < 0)
            {
                moveDirection.y = -2f;
                characterController.stepOffset = originalStepOffset;
            }
            else if (!isGrounded)
            {
                characterController.stepOffset = 1;
            }

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (!canMove)
            {
                horizontal = 0.0f;
                vertical = 0.0f;
            }
            Vector3 inputDirection = transform.right * horizontal + transform.forward * vertical;
            inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

            float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            targetSpeed = isCrouching ? crouchSpeed : targetSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);

            if (inputDirection.magnitude > 0.1f)
            {
                float accelerationRate = isGrounded ? acceleration : acceleration * airControl;
                currentVelocity = Vector3.Lerp(currentVelocity, inputDirection * targetSpeed, accelerationRate * Time.deltaTime);
            }
            else
            {
                float decelerationRate = isGrounded ? deceleration : deceleration * airControl;
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, decelerationRate * Time.deltaTime);
            }

            if (Mathf.Abs(horizontal) > 0.01f)
                tilt = (horizontal * tiltAmount) * -1;
            else
                tilt = 0;

            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            //characterController.Move(move * currentSpeed * Time.deltaTime);
            characterController.Move(currentVelocity * Time.deltaTime);
        }

        private void HandleMouseLook()
        {
            if (!canLookAround)
                return;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            yRotation += mouseX;


            // Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, tilt);
            // playerCamera.localRotation = Quaternion.Lerp(
            //     playerCamera.localRotation, 
            //     targetRotation, 
            //     tiltSpeed * Time.deltaTime
            // );
            // transform.Rotate(Vector3.up * mouseX);

            playerCamera.localRotation = Quaternion.Euler(xRotation + xShakeRotation, 0, tilt);
            transform.localRotation = Quaternion.Euler(0, yRotation + yShakeRotation, 0);
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded && canMove/*&& !isCrouching*/)
            {
                moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                moveDirection = moveDirection.normalized * Mathf.Max(moveDirection.magnitude, walkSpeed);
            }

            moveDirection.y += gravity * Time.deltaTime;
        }

        private void HandleCrouch()
        {

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (Physics.Raycast(footstepOrigin.position + new Vector3(0, crouchHeight, 0), Vector3.up, out RaycastHit hit))
                {
                    if (hit.distance < standHeight - crouchHeight)
                    {
                        return;
                    }
                    else
                    {
                        isCrouching = !isCrouching;
                    }
                }
                else
                {
                    isCrouching = !isCrouching;
                }
                
            }

            float targetHeight = isCrouching ? crouchHeight : standHeight;
            currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);

            characterController.height = currentHeight;
            characterController.center = new Vector3(0, currentHeight / 2, 0);

            Vector3 camPos = playerCamera.localPosition;
            camPos.y = defaultCameraY - (standHeight - currentHeight);
            playerCamera.localPosition = camPos;
        }

        private void HandleHeadBob()
        {
            if (!isGrounded) return;

            float speed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;
            if (speed < 0.1f)
            {
                headBobTimer = 0;
                return;
            }

            float bobAmount = Input.GetKey(KeyCode.LeftShift) ? runBobAmount : walkBobAmount;
            float bobSpeed = Input.GetKey(KeyCode.LeftShift) ? runBobSpeed : walkBobSpeed;

            headBobTimer += Time.deltaTime * bobSpeed;
            float bobY = Mathf.Sin(headBobTimer) * bobAmount;
            float bobX = Mathf.Cos(headBobTimer * 0.5f) * bobAmount * 2f;

            Vector3 camPos = playerCamera.localPosition;
            camPos.y = defaultCameraY - (standHeight - currentHeight) + bobY;
            camPos.x = bobX;
            playerCamera.localPosition = camPos;
        }

        private void HandleFootsteps()
        {
            if (!isGrounded || characterController.velocity.magnitude < 0.1f)
            {
                footstepTimer = 0;
                return;
            }

            footstepTimer += Time.deltaTime;
            float interval = Input.GetKey(KeyCode.LeftShift) ? footstepInterval * 0.6f : footstepInterval;

            if (footstepTimer >= interval)
            {
                PlayFootstepSound();
                footstepTimer = 0;
            }
        }

        private void PlayFootstepSound()
        {
            if (footstepSounds.Length == 0) return;

            string currentSurface = GetSurfaceTag();

            AudioClip clip = GetFootstepClip(currentSurface);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }

        private string GetSurfaceTag()
        {
            if (Physics.Raycast(footstepOrigin.position, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
            {
                return hit.collider.tag;
            }
            else
            {
                return "Default";
            }
        }

        private AudioClip GetFootstepClip(string surfaceTag)
        {
            foreach (var surface in footstepSounds)
            {
                if (surface.SurfaceTag == surfaceTag && surface.FootstepClips.Length > 0)
                {
                    return surface.FootstepClips[Random.Range(0, surface.FootstepClips.Length)];
                }
            }

            foreach (var surface in footstepSounds)
            {
                if (surface.SurfaceTag == "Default" && surface.FootstepClips.Length > 0)
                {
                    return surface.FootstepClips[Random.Range(0, surface.FootstepClips.Length)];
                }
            }
            return null;
        }

        private void HandleBodyPhysics()
        {
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, crouchHeight / 2, 0);
            currentHeight = Mathf.Lerp(currentHeight, crouchHeight, crouchTransitionSpeed * Time.deltaTime);
            Vector3 camPos = playerCamera.localPosition;
            camPos.y = defaultCameraY - (standHeight - currentHeight);
            playerCamera.localPosition = camPos;
            Quaternion targetRotation = Quaternion.Euler(-60f, 0f, 30f);
            playerCamera.localRotation = Quaternion.Lerp(playerCamera.localRotation, targetRotation, tiltSpeed * Time.deltaTime);

            moveDirection.y += gravity * Time.deltaTime;
        }

        private void ApplyFinalMovements()
        {
            if (!usePhysics)
                return;
            characterController.Move(moveDirection * Time.deltaTime);
        }

        public void ShakeCamera(float duration)
        {
            ShakeCamera(duration, 0.02f);
        }

        public void ShakeCamera(float duration, float magnitude)
        {
            StartCoroutine(CameraShake(duration, magnitude));
        }
        public void ShakeCameraRotation(float duration, float magnitude)
        {
            if (playerCamera != null)
            {
                StartCoroutine(CameraRotationShake(duration, magnitude));
            }
        }

        private IEnumerator CameraShake(float duration, float magnitude)
        {
            Vector3 originalPos = playerCamera.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                playerCamera.localPosition = new Vector3(
                    originalPos.x + x,
                    originalPos.y + y,
                    originalPos.z
                );

                elapsed += Time.deltaTime;
                yield return null;
            }
            playerCamera.localPosition = originalPos;
        }
        private IEnumerator CameraRotationShake(float duration, float magnitude)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                xShakeRotation = /*Random.Range(0f, 1f) * */elapsed * magnitude;
                yShakeRotation = /*Random.Range(0f, 1f) * */ elapsed * magnitude;
                // можно добавить тряску по Z, но она не нужна для FPS
                // float zRotation = Random.Range(-1f, 1f) * magnitude;

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        public void FreezeAllMovement()
        {
            FreezeMovement(false, false);
        }
        public void UnfreezeAll()
        {
            FreezeMovement(true, true);
        }

        public void FreezeMovement(bool canMove, bool canLookAround = true)
        {
            this.canMove = canMove;
            this.canLookAround = canLookAround;
            
            // moveDirection = Vector3.zero;
            // currentVelocity = Vector3.zero;
        }
        public void FreezeMovement(bool canMove, bool canLookAround, float duration)
        {
            StartCoroutine(FreezeActionsForSeconds(canMove, canLookAround, duration));
        }

        IEnumerator FreezeActionsForSeconds(bool canMoveNow, bool canLookAroundNow, float duration)
        {
            float elapsed = 0.0f;
            FreezeMovement(canMoveNow, canLookAroundNow);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            FreezeMovement(true, true);
        }
        public void OnEnablePhysics(bool usePhysics)
        {
            this.usePhysics = usePhysics;
        }

        public void SetRotationAsGO()
        {
            yRotation = transform.localRotation.eulerAngles.y;
            xRotation = transform.localRotation.eulerAngles.x;
        }

        public void KillPlayer()
        {
            isDead = true;
            FreezeMovement(false, false);
            ShakeCamera(0.2f, 0.02f);
        }

        public void LoadData(GameData data)
        {
            isLoading = true;
            characterController.enabled = false;

            xRotation = data.PlayerRotation.x;
            yRotation = data.PlayerRotation.y;
            transform.position = data.PlayerPosition;
            canMove = data.CanMove;
            canLookAround = data.CanLookAround;
            usePhysics = data.UsePhysics;
            isDead = data.IsDead;
            playerCamera.localRotation = Quaternion.Euler(data.CameraRotation);

            characterController.enabled = true;
            isLoading = false;

        }

        public void SaveData(ref GameData data)
        {
            isLoading = true;

            data.PlayerRotation = new Vector3(xRotation, yRotation, transform.rotation.eulerAngles.z);
            data.PlayerPosition = transform.position;
            data.CanMove = canMove;
            data.CanLookAround = canLookAround;
            data.UsePhysics = usePhysics;
            data.CameraRotation = playerCamera.localRotation.eulerAngles;
            isLoading = false;
        }

        [System.Serializable]
        public struct SurfaceFootsteps
        {
            public string SurfaceTag;
            public AudioClip[] FootstepClips;
        }

    }
}