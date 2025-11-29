using System.Diagnostics;
using UnityEngine;
public class Pickups : MonoBehaviour
{
    public enum PickupType
    {
        Life = 0,
        Score = 1,
        HP = 2,
        Victory = 3,
        Weapon = 4
    }

    public PickupType pickupType = PickupType.Life; // Type of the pickup
    public GameObject weaponPrefab; // Weapon prefab if pickupType is Weapon
    private bool isTwoHanded = false;
    private bool playerInRange = false;
    private PlayerMovement player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerMovement>();
            player.pickupInRange = this;
            playerInRange = true;

            UI_Prompt.Instance.Show("Interact");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player != null)
                player.pickupInRange = null;

            player = null;
            playerInRange = false;

            UI_Prompt.Instance.Hide();
        }
    }

    public void Collect()
    {
        if (!playerInRange || player == null) return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        PlayerWeaponController weaponController = player.GetComponent<PlayerWeaponController>();

        switch (pickupType)
        {
            case PickupType.Life:
                GameManager.Instance.AddLife(1);
                UnityEngine.Debug.Log("Collected LIFE → +1 life");
                break;

            case PickupType.Score:
                GameManager.Instance.AddScore(1);
                UnityEngine.Debug.Log("Collected SCORE → +1 point");
                break;

            case PickupType.HP:
                if (health != null)
                {
                    health.Heal(20f);
                    UnityEngine.Debug.Log($"Collected HP → Current HP: {health.GetHealthPercent() * 100f}%");
                }
                else
                {
                    UnityEngine.Debug.LogError("ERROR: PlayerHealth NOT FOUND on player!");
                }
                break;

            case PickupType.Victory:
                GameManager.Instance.SetState(GameManager.GameState.Victory);
                UnityEngine.Debug.Log("Collected VICTORY → You win!");
                break;

            case PickupType.Weapon:
                if (weaponController != null && weaponPrefab != null)
                {
                    weaponController.EquipWeapon(weaponPrefab, isTwoHanded);
                    UnityEngine.Debug.Log($"Collected WEAPON → Equipped {(isTwoHanded ? "2H" : "1H")} weapon");

                    UI_Prompt.Instance.Hide();
                    return;
                }
                else
                {
                    UnityEngine.Debug.LogError("ERROR: WeaponController or weaponPrefab missing!");
                }
                break;

        }

        var data = SaveSystem.Load() ?? new SaveData();
        data.Player.Lives = GameManager.Instance.Lives;
        data.Score = GameManager.Instance.Score;

        if (health != null) 
            data.Player.CurrentHealth = health.GetCurrentHealth();

        if (weaponPrefab != null)
        {
            data.Player.EquippedWeaponId = weaponPrefab.name; // or a custom ID
            data.Player.IsTwoHanded = isTwoHanded;
        }

        SaveSystem.Save(data);

        UI_Prompt.Instance.Hide();
        Destroy(transform.parent.gameObject);

    }
}

