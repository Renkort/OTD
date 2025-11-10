using System;
using UnityEngine;

namespace Akkerman.InteractionSystem
{
    
    public class InteractionPoint : MonoBehaviour, IInteractable
    {
        public Action OnInteract { get; set; }
        private string interactText = string.Empty;
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out FPS.Player player))
            {
                player.Interactable = this;
                FPS.Player.Instance.ToggleInteractText(true, interactText);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out FPS.Player player))
            {
                if (player.Interactable is InteractionPoint interactionPoint && interactionPoint == this)
                {
                    player.Interactable = null;
                    FPS.Player.Instance.ToggleInteractText(false);
                }
            }
        }

        public void SetInteractText(string newText)
        {
            interactText = $"[E] {newText}";
        }

        public void Interact(FPS.Player player)
        {
            FPS.Player.Instance.ToggleInteractText(false, interactText);
            OnInteract?.Invoke();
        }
    }
}
