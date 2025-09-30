using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]

public class InteractableObject : MonoBehaviour, IInteractable
{
    [HideInInspector] public string InteractText;
    public Outline outline;
    public UnityEvent InteractAction;

    void Awake()
    {
        SetOutline(false);
    }

    public void SetOutline(bool isActive)
    {
        outline.enabled = isActive;
    }

    public void Interact(Player player)
    {
        player.CurrentInteractable = this;
        InteractAction?.Invoke();
    }
}
