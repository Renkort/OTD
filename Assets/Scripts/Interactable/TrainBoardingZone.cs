using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.InteractionSystem
{
    
    public class TrainBoardingZone : MonoBehaviour
    {
        [SerializeField] private TrainMovement trainMovement;
        [SerializeField] UnityEvent OnPlayerBoards, OnPlayerExits;
        private GameObject currentPlayer;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && currentPlayer == null)
            {
                currentPlayer = other.gameObject;
                trainMovement.PlayerBoardsTrain(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.gameObject == currentPlayer)
            {
                trainMovement.PlayerExitsTrain();
                currentPlayer = null;
            }
        }
    }
}