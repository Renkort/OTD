using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour, IDataPersistance
{
    [SerializeField] private FPSPlayerController fpsController;
    [field: SerializeField] public DialogueUI DialogueUI { get; set; }
    [field: SerializeField] public CutsceneHandler CutsceneHandler { get; set; }

    [SerializeField] private TextMeshProUGUI interactTextLabel;
    [SerializeField] private Light flashlight;
    [SerializeField] private GameUI gameUI;
    private bool isFlashlightActive = false;
    public IInteractable Interactable { get; set; }
    public static Player Instance;
    [HideInInspector] public bool IsDead = false;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ToggleInteractText(false);
        FreezePlayerActions(true, true, 1.3f);
    }
    private void Update()
    {
        if (DialogueUI.IsOpen)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable?.Interact(this);
        }
        
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
        Debug.Log($"DEBUG: PLAYER IS DEAD!");
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
