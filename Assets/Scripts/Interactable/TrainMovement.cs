using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.InteractionSystem
{
    
    public class TrainMovement : MonoBehaviour
    {
        [Header("Station Points")]
        public Transform[] stations;
        private int currentStationIndex = 0;

        [Header("Movement Settings")]
        public float journeyTime = 10f;
        public float stopTime = 5f;
        private bool isMoving = false;
        private bool isWaiting = false;

        [Header("Events")]
        public UnityEvent OnTrainDeparture;
        public UnityEvent OnTrainArrival;
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource doorAudio;

        [Header("Player Settings")]
        public Transform playerEntryPoint;
        private Transform playerTransform;
        // private bool playerIsOnBoard = false;
        private CharacterController playerCharacterController;
        private Vector3 previousTrainPosition;


        void Start()
        {
            OnTrainArrival.AddListener(OpenDoor);
            OnTrainDeparture.AddListener(CloseDoor);

            if (stations.Length > 1)
            {
                MoveToNextStationAfterStop();
            }
        }

        public void PlayerBoardsTrain(Transform player)
        {
            playerTransform = player;
            // playerIsOnBoard = true;

            player.SetParent(transform);

            if (playerEntryPoint != null)
            {
                player.position = playerEntryPoint.position;
                player.rotation = playerEntryPoint.rotation;
            }
        }

        public void PlayerExitsTrain()
        {
            if (playerTransform != null)
            {
                playerTransform.SetParent(null);
                // playerIsOnBoard = false;
            }
        }


        public void TriggerDeparture()
        {
            if (!isMoving && !isWaiting)
            {
                MoveToNextStationAfterStop();
            }
        }

        private void MoveToNextStationAfterStop()
        {
            if (currentStationIndex < stations.Length - 1)
            {
                StartCoroutine(StopAndGoRoutine());
            }
            else
            {
                OnTrainArrival?.Invoke();
                Debug.Log("The final station.");
            }
        }

        private IEnumerator StopAndGoRoutine()
        {
            if (currentStationIndex > 0)
            {
                isWaiting = true;
                yield return new WaitForSeconds(stopTime);
                isWaiting = false;
            }

            currentStationIndex++;
            StartCoroutine(MoveToStation(stations[currentStationIndex]));
        }

        private IEnumerator MoveToStation(Transform targetStation)
        {
            isMoving = true;
            OnTrainDeparture?.Invoke();

            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            float elapsedTime = 0f;

            while (elapsedTime < journeyTime)
            {
                float t = elapsedTime / journeyTime;
                float smoothT = SmoothEaseInOut(t);

                transform.position = Vector3.Lerp(startPos, targetStation.position, smoothT);
                transform.rotation = Quaternion.Lerp(startRot, targetStation.rotation, smoothT);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetStation.position;
            transform.rotation = targetStation.rotation;

            isMoving = false;
            OnTrainArrival?.Invoke();
            // MoveToNextStationAfterStop();
        }

        private float SmoothEaseInOut(float t)
        {
            return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
        }

        private void OpenDoor()
        {
            animator.SetBool("IsOpen", true);
            doorAudio.Play();
        }

        private void CloseDoor()
        {
            animator.SetBool("IsOpen", false);
            doorAudio.Play();
        }
    }
}