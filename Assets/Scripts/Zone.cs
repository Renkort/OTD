using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        Debug.Log($"DEBUG: Zone: {gameObject.name}, WasTriggered: {wasTriggered}");
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
