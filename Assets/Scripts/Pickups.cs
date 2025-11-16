using System.Diagnostics;
using UnityEngine;
public class Pickups : MonoBehaviour
{
    public enum PickupType
    {
        Life = 0,
        Score = 1,
        HP = 2
    }

    public PickupType pickupType = PickupType.Life; // Type of the pickup
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            pm.pickupInRange = this;

            UI_Prompt.Instance.Show("Interact");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            pm.pickupInRange = null;

            UI_Prompt.Instance.Hide();
        }
    }

    public void Collect(PlayerMovement pc)
    {
        if (!playerInRange) return;

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
                PlayerHealth health = pc.GetComponent<PlayerHealth>();
                health.Heal(20f);
                UnityEngine.Debug.Log($"Collected HP → Current HP: {health.GetHealthPercent() * 100f}%");
                break;
        }

        UI_Prompt.Instance.Hide();
        Destroy(gameObject);
    }
}
