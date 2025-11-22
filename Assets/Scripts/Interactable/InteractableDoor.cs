using UnityEngine;
using UnityEngine.Events;
using Akkerman.SaveSystem;

namespace Akkerman.InteractionSystem
{
    public class InteractableDoor : InteractableObject, IDataPersistance
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
            // OnInteract += OpenCloseDoor;
            // SetInteractText(OpenActionText);
            InteractText = OpenActionText;
            InteractAction.AddListener(OpenCloseDoor);
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
            }
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
                //SetInteractText(OpenActionText);
                InteractText = OpenActionText;
            }
            else
            {
                audioSource.clip = openSFX;
                onOpenDoor?.Invoke();
                isOpen = true;
                animator.SetTrigger("Open");
                //SetInteractText(CloseActionText);
                InteractText = CloseActionText;
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
}
