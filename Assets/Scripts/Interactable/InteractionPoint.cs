using System;
using UnityEngine;

public class InteractionPoint : MonoBehaviour, IInteractable
{
    public Action OnInteract { get; set; }
    private string interactText = string.Empty;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            player.Interactable = this;
            Player.Instance.ToggleInteractText(true, interactText);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            if (player.Interactable is InteractionPoint interactionPoint && interactionPoint == this)
            {
                player.Interactable = null;
                Player.Instance.ToggleInteractText(false);
            }
        }
    }

    public void SetInteractText(string newText)
    {
        interactText = $"[E] {newText}";
    }

    public void Interact(Player player)
    {
        Player.Instance.ToggleInteractText(false, interactText);
        OnInteract?.Invoke();
    }
}
