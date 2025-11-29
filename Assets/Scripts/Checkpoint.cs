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
        var cpPos = transform.position;
        data.Checkpoint.checkpointPosition = new float[] { cpPos.x, cpPos.y, cpPos.z };
        data.Checkpoint.HasCheckpoint = true;

        // store current lives from GameManager if available:
        var gm = UnityEngine.Object.FindFirstObjectByType<GameManager>();
        var player = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
        if (gm != null) data.Player.Lives = gm.Lives;

        if (gm != null)
        {
            Vector3 playerPos = player.transform.position;
            data.Player.Position = new float[] { playerPos.x, playerPos.y, playerPos.z }; // optional
            data.Player.Rotation = new QuaternionData(player.transform.rotation);

            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
                data.Player.CurrentHealth = health.GetCurrentHealth();

            var weaponController = player.GetComponent<PlayerWeaponController>();
            if (weaponController != null && weaponController.CurrentWeaponPrefab != null)
            {
                data.Player.EquippedWeaponId = weaponController.CurrentWeaponPrefab.name;
                data.Player.IsTwoHanded = weaponController.IsTwoHanded;
            }
        }

        SaveSystem.Save(data);
    }
}