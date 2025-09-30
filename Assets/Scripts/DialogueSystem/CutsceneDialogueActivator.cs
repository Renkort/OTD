using System.Collections;
using UnityEngine;

namespace DialogueSystem
{
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
            yield return new WaitForSeconds(0.5f);

            isPlayed = true;
            Player player = FindObjectOfType<Player>();
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
