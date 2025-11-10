using System.Collections;
using UnityEngine;

namespace Akkerman.DialogueSystem
{
    public class CutsceneDialogueActivator2D : DialogueActivator2D
    {
        [SerializeField] private bool playOnAwake;
        private bool isPlayed = false;

        private void Start()
        {
            if (playOnAwake && !isPlayed)
            {
                StartCoroutine(PlayOnAwake());
            }
        }

        private IEnumerator PlayOnAwake()
        {
            yield return new WaitForSeconds(0.5f);

            isPlayed = true;
            FPS.Player player = FindObjectOfType<FPS.Player>();
            //player.Interactable = this;
            Interact(player);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isPlayed)
                return;
            if (collision.CompareTag("Player") && collision.TryGetComponent(out FPS.Player player))
            {
                isPlayed = true;
                player.Interactable = this;
                Interact(player);
            }
        }
    }
}
