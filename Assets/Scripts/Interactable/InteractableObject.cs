using UnityEngine;
using UnityEngine.Events;


namespace Akkerman.InteractionSystem
{
    
    [RequireComponent(typeof(Outline))]

    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [HideInInspector] public string InteractText;
        public Outline outline;
        public UnityEvent InteractAction;
        [SerializeField] private LayerMask interactionLayer;

        void Awake()
        {
            SetOutline(false);
            if (1 << gameObject.layer != interactionLayer)
                Debug.LogWarning($"{gameObject} is not on interaction layer");
        }

        public void SetOutline(bool isActive)
        {
            outline.enabled = isActive;
        }

        public void Interact(Akkerman.FPS.Player player)
        {
            player.CurrentInteractable = this;
            InteractAction?.Invoke();
        }
    }
}
