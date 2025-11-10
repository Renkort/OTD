using UnityEngine;
using UnityEngine.Events;
using Akkerman.SaveSystem;
using Akkerman.FPS;

namespace Akkerman.InteractionSystem
{
    
    [RequireComponent(typeof(Collider))]
    public class Zone : MonoBehaviour, IDataPersistance
    {
        [SerializeField] private string Id;
        [ContextMenu("Generate guid for id")]
        private void GenerateGuid()
        {
            Id = System.Guid.NewGuid().ToString();
        }
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private bool playOnceOnEnter;
        private bool wasTriggered = false;
        public bool IsPlayerInside { get; private set; }
        public UnityEvent OnPlayerEnter, OnPlayerExit;
        void OnTriggerEnter(Collider other)
        {
            if (wasTriggered)
                return;
            if (other.CompareTag("Player") && other.gameObject.TryGetComponent(out Player player))
            {
                IsPlayerInside = true;
                OnPlayerEnter?.Invoke();
                if (playOnceOnEnter)
                    wasTriggered = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.gameObject.TryGetComponent(out Player player))
            {
                IsPlayerInside = false;
                OnPlayerExit?.Invoke();
            }
        }
        public void LoadData(GameData data)
        {
            data.ActiveZones.TryGetValue(Id, out wasTriggered);
        }

        public void SaveData(ref GameData data)
        {
            if (!data.ActiveZones.ContainsKey(Id))
            {
                data.ActiveZones.Add(Id, wasTriggered);
            }
            else
            {
                data.ActiveZones[Id] = wasTriggered;
            }
        }

    }
}
