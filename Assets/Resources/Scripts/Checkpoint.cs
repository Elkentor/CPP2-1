using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string checkpointId = "CP_1"; // unique id per checkpoint
    public bool autoActivateOnTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!autoActivateOnTrigger) return;
        if (!other.CompareTag("Player")) return;

        ActivateCheckpoint();
    }

    public void ActivateCheckpoint()
    {
        Debug.Log($"Checkpoint {checkpointId} activated.");

        GameManager.Instance.HasCheckpoint = true;
        GameManager.Instance.LastCheckpointPosition = transform.position;
        GameManager.Instance.SavePrototypeState();
    }

}
