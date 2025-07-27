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
    private bool isFlashlightActive = false;
    public IInteractable Interactable { get; set; }
    public static Player Instance;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ToggleInteractText(false);
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

    public void ToggleInteractText(bool state, string text = "")
    {
        interactTextLabel.gameObject.SetActive(state);
        interactTextLabel.text = text;
    }

    public void Kill()
    {
        Debug.Log($"DEBUG: PLAYER IS DEAD!");
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

    }
    public void SaveData(ref GameData data)
    {
        
    }
}
