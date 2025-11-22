using Akkerman.DialogueSystem;
using UnityEngine;
using TMPro;
using System;
using Akkerman.CutsceneSystem;
using Akkerman.InteractionSystem;
using Akkerman.InventorySystem;
using Akkerman.SaveSystem;
using Akkerman.UI;

namespace Akkerman.FPS
{
    
    [RequireComponent(typeof(AudioSource))]
    public class Player : MonoBehaviour, IDataPersistance
    {
        public ActionFPSController fpsController;
        [field: SerializeField] public DialogueUI DialogueUI { get; set; }
        [field: SerializeField] public CutsceneHandler CutsceneHandler { get; set; }

        [SerializeField] private TextMeshProUGUI interactTextLabel;
        [SerializeField] private float maxInteractionDistance = 4f;
        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private Light flashlight;
        [SerializeField] private GameUI gameUI;
        [Header("AUDIO")]
        [SerializeField] private AudioClip deathSFX;
        [Header("DEBUG")]
        [SerializeField] private bool isDebugMode = false;
        [SerializeField] private Transform startPosition;

        public event Action OnPlayerDied;
        public IInteractable Interactable { get; set; }
        public InteractableObject CurrentInteractable { get; set; }
        public static Player Instance;
        [HideInInspector] public bool IsDead = false;
        private Ray viewRay;
        private GameObject lastRaycastObject;
        public static float StartFreezeTime = 1.3f;

        void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            ToggleInteractText(false);
            FreezePlayerActions(true, true, StartFreezeTime);
            if (isDebugMode && startPosition != null)
            {
                transform.position = startPosition.position;
            }
        }
        private void Update()
        {
            if (DialogueUI.IsOpen)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.E) && CurrentInteractable != null)
            {
                //Interactable?.Interact(this);
                CurrentInteractable.SetOutline(false);
                CurrentInteractable.Interact(this);
            }

            HighlightInteractables();
        }

        public void FreezePlayerActions(bool isFreezedMovement, bool isFreezedLook)
        {
            fpsController.FreezeMovement(!isFreezedMovement, !isFreezedLook);
        }

        public void FreezePlayerActions(bool isFreezedMovement, bool isFreezedLook, float duration)
        {
            fpsController.FreezeMovement(!isFreezedMovement, !isFreezedLook, duration);
        }

        public void OnEnablePhysics(bool isActive)
        {
            fpsController.OnEnablePhysics(isActive);
        }

        public void ToggleInteractText(bool state, string text = "")
        {
            interactTextLabel.gameObject.SetActive(state);
            interactTextLabel.text = text;
        }

        public void Kill()
        {
            GetComponent<AudioSource>().PlayOneShot(deathSFX);
            IsDead = true;
            gameUI.IngameUI.ShowDeathScreen();
            fpsController.KillPlayer();
            OnPlayerDied?.Invoke();
        }

        public void SetCursorVisible(bool isVisible)
        {
            if (isVisible)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = isVisible;
        }

        private void HighlightInteractables()
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            viewRay = ray;
            // Debug.DrawRay(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), new Vector3(0, 0, maxInteractionDistance), Color.blue);

            if (Physics.Raycast(ray, out hit, maxInteractionDistance/*, interactionLayer*/))
            {
                GameObject objectHitByRaycast = hit.transform.gameObject;
                if (objectHitByRaycast == lastRaycastObject)
                {
                    return;
                }
                else if (1 << objectHitByRaycast.layer != interactionLayer)
                {
                    CurrentInteractable?.SetOutline(false);
                    ToggleInteractText(false);
                    CurrentInteractable = null;
                    lastRaycastObject = null;
                    
                    return;
                }
                if (lastRaycastObject)
                    lastRaycastObject.GetComponentInParent<InteractableObject>()?.SetOutline(false);
                lastRaycastObject = objectHitByRaycast;

                CurrentInteractable = objectHitByRaycast.gameObject.GetComponentInParent<InteractableObject>();
                CurrentInteractable.SetOutline(true);
                ToggleInteractText(true, CurrentInteractable.InteractText);
            }
            else if (CurrentInteractable)
            {
            }
        }

        public void LoadData(GameData data)
        {
            IsDead = data.IsDead;

        }
        public void SaveData(ref GameData data)
        {
            data.IsDead = IsDead;
        }
    }
}
