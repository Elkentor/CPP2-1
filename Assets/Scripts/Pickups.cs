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
                pc.Lives++;
                Debug.Log("Life collected! Current lives: " + pc.Lives);
                break;

            case PickupType.Score:
                pc.Score++;
                Debug.Log("Score collected! Current score: " + pc.Score);
                break;

            case PickupType.HP:
                pc.currentHealth = Mathf.Min(pc.currentHealth + 20, pc.maxHealth);
                Debug.Log("HP collected! Current Lives: " + pc.currentHealth);
                break;
        }

        UI_Prompt.Instance.Hide();
        Destroy(gameObject);
    }
}
