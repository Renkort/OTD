using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointActivator : MonoBehaviour
{
    public string Id { get; private set; }

    public void TriggerCheckpoint()
    {
        Debug.Log($"Checkpoint...");
        DataPersistenceManager.Instance.SaveGame();
    }
}
