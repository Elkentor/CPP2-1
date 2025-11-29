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

        // Save to SaveSystem immediately
        var data = SaveSystem.Load() ?? new SaveData();

        // store checkpoint id and transform position
        data.Checkpoint.checkpointId = checkpointId;
        var pos = transform.position;
        data.Checkpoint.checkpointPosition = new float[] { pos.x, pos.y, pos.z };

        // store current lives from GameManager if available:
        var gm = UnityEngine.Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            data.Player.Lives = gm.Lives;
            data.Player.Position = new float[] { pos.x, pos.y, pos.z }; // optional
        }

        SaveSystem.Save(data);
    }
}