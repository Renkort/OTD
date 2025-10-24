using UnityEngine;

public class CheckpointActivator : MonoBehaviour
{
    [SerializeField] private Collider triggerCollider;
    public string Id { get; private set; }

    public void TriggerCheckpoint()
    {
        Debug.Log($"Checkpoint...");
        triggerCollider.enabled = false;
        DataPersistenceManager.Instance.SaveGame();
    }
}
