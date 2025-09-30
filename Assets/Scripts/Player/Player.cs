using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour, IDataPersistance
{
    [SerializeField] private FPSPlayerController fpsController;
    [field: SerializeField] public DialogueUI DialogueUI { get; set; }
    [field: SerializeField] public CutsceneHandler CutsceneHandler { get; set; }

    [SerializeField] private TextMeshProUGUI interactTextLabel;
    [SerializeField] private float maxInteractionDistance = 4f;
    [SerializeField] private Light flashlight;
    [SerializeField] private GameUI gameUI;
    [Header("AUDIO")]
    [SerializeField] private AudioClip deathSFX;
    [Header("DEBUG")]
    [SerializeField] private bool isDebugMode = false;
    [SerializeField] private Transform startPosition;

    private bool isFlashlightActive = false;
    public IInteractable Interactable { get; set; }
    public InteractableObject CurrentInteractable { get; set; }
    public static Player Instance;
    [HideInInspector] public bool IsDead = false;
    private Ray viewRay;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ToggleInteractText(false);
        FreezePlayerActions(true, true, 1.3f);
        if (isDebugMode && startPosition != null)
        {
            transform.position = startPosition.position;
        }
    }
    private void Update()
    {
        if (DialogueUI.IsOpen || InventoryUI.Instance.IsOpen)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && CurrentInteractable != null)
        {
            //Interactable?.Interact(this);
            CurrentInteractable.Interact(this);
        }

        HighlightInteractables();
        HandleInventoryInput();
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

    private void HandleInventoryInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isFlashlightActive)
            {
                isFlashlightActive = false;
                flashlight.gameObject.SetActive(false);
            }
            else
            {
                isFlashlightActive = true;
                flashlight.gameObject.SetActive(true);
            }
        }
    }

    private void HighlightInteractables()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        viewRay = ray;
        // Debug.DrawRay(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), new Vector3(0, 0, maxInteractionDistance), Color.blue);

        if (Physics.Raycast(ray, out hit, maxInteractionDistance))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;
            if (objectHitByRaycast.GetComponent<InteractableObject>() == CurrentInteractable)
                return;

            if (objectHitByRaycast.GetComponent<InteractableObject>())
            {
                CurrentInteractable = objectHitByRaycast.gameObject.GetComponent<InteractableObject>();
                CurrentInteractable.SetOutline(true);
                ToggleInteractText(true, CurrentInteractable.InteractText);
            }
            else
            {
                if (CurrentInteractable)
                {
                    CurrentInteractable.SetOutline(false);
                    ToggleInteractText(false);
                    CurrentInteractable = null;
                }
            }
        }
        else if (CurrentInteractable)
        {
            CurrentInteractable.SetOutline(false);
            ToggleInteractText(false);
        }
    }

    public void LoadData(GameData data)
    {
        transform.position = data.PlayerPosition;
        isFlashlightActive = data.IsFlashlightActive;
        flashlight.gameObject.SetActive(isFlashlightActive);
        IsDead = data.IsDead;
        transform.localRotation = Quaternion.Euler(data.PlayerRotation);

    }
    public void SaveData(ref GameData data)
    {
        data.PlayerPosition = transform.position;
        data.IsFlashlightActive = isFlashlightActive;
        data.IsDead = IsDead;
        Quaternion playerRotation = transform.rotation;
        data.PlayerRotation = new Vector3(playerRotation.x, playerRotation.y, playerRotation.z);
    }
}
