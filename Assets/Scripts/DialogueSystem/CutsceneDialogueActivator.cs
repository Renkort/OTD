using System.Collections;
using Akkerman.FPS;
using UnityEngine;

namespace Akkerman.DialogueSystem
{
    [RequireComponent(typeof(Collider))]
    public class CutsceneDialogueActivator : DialogueActivator3D
    {
        [SerializeField] private bool playOnAwake;
        private bool isPlayed = false;

        private void Start()
        {
            InteractAction.AddListener(ActivateDialogue);
            if (playOnAwake && !isPlayed)
            {
                StartCoroutine(PlayOnAwake());
            }
        }

        private IEnumerator PlayOnAwake()
        {
            yield return new WaitForSeconds(Player.StartFreezeTime + 0.05f);

            isPlayed = true;
            Player player = FindFirstObjectByType<Player>();
            Interact(player);
        }
        private void OnTriggerEnter(Collider collision)
        {
            if (isPlayed)
                return;
            if (collision.CompareTag("Player") && collision.TryGetComponent(out Player player))
            {
                isPlayed = true;
                player.CurrentInteractable = this;
                Interact(player);
            }
        }
    }
}
