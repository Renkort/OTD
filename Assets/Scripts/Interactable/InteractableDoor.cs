using UnityEngine;
using UnityEngine.Events;

public class InteractableDoor : InteractionPoint, IDataPersistance
{
    [SerializeField] private string Id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        Id = System.Guid.NewGuid().ToString();
    }
    [SerializeField] private UnityEvent onOpenDoor;
    [SerializeField] private Animator animator;
    [SerializeField] private string OpenActionText;
    [SerializeField] private string CloseActionText;
    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip closeSFX;
    [SerializeField] private AudioClip lockedSFX;
    [SerializeField] private AudioSource audioSource;
    public bool IsLocked = false;

    private bool isOpen = false;
    void Start()
    {
        OnInteract += OpenCloseDoor;
        SetInteractText(OpenActionText);
        if (Id == string.Empty)
            Debug.LogError($"Door {gameObject.name} must have uniq Id");
    }

    public void LockUnlock(bool isLocked)
    {
        this.IsLocked = isLocked;
        if (isLocked && isOpen)
        {
            isOpen = false;
            animator.SetTrigger("Close");
            SetInteractText(OpenActionText);
        }
        //play sound
    }

    private void OpenCloseDoor()
    {
        if (IsLocked)
        {
            audioSource.clip = lockedSFX;
            audioSource.Play();
            return;
        }

        if (isOpen)
        {
            audioSource.clip = closeSFX;
            isOpen = false;
            animator.SetTrigger("Close");
            SetInteractText(OpenActionText);
        }
        else
        {
            audioSource.clip = openSFX;
            onOpenDoor?.Invoke();
            isOpen = true;
            animator.SetTrigger("Open");
            SetInteractText(CloseActionText);
        }
        audioSource.Play();
    }

    public void LoadData(GameData data)
    {
        /*if (data.IsOpenDoors.ContainsKey(Id))
            isOpen = data.IsOpenDoors[Id];
        else
            Debug.LogWarning($"Door id: {Id} does not have save state in game data");
        */
        data.IsLockedDoors.TryGetValue(Id, out IsLocked);
        
    }
    public void SaveData(ref GameData data)
    {
        data.IsLockedDoors[Id] = IsLocked;
        if (data.IsLockedDoors.ContainsKey(Id))
        {
            data.IsLockedDoors.Remove(Id);
        }
        data.IsLockedDoors.Add(Id, IsLocked);
    }
}
