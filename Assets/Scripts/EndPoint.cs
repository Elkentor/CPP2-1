using UnityEngine;

public class EndPointTrigger : MonoBehaviour
{
    public GameObject quitMenuUI; // Assign your UI panel in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quitMenuUI.SetActive(true); // Show the quit option
            Time.timeScale = 0f; // Optional: pause the game
        }
    }
}