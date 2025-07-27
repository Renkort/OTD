using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Zone : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool playOnceOnEnter;
    public bool IsPlayerInside { get; private set; }
    public UnityEvent OnPlayerEnter, OnPlayerExit;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.TryGetComponent(out Player player))
        {
            IsPlayerInside = true;
            OnPlayerEnter?.Invoke();
            if (playOnceOnEnter)
                Destroy(this);
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
}
