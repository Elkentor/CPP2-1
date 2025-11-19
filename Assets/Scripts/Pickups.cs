using System.Diagnostics;
using UnityEngine;
public class Pickups : MonoBehaviour
{
    public enum PickupType
    {
        Life = 0,
        Score = 1,
        HP = 2,
        Victory = 3
    }

    public PickupType pickupType = PickupType.Life; // Type of the pickup
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
        }

        UI_Prompt.Instance.Hide();
        Destroy(transform.parent.gameObject);
    }
}

