using UnityEngine;

public class EndPointTrigger : QuitGame
{
    public GameObject quitMenuUI; // Assign your UI panel in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quitMenuUI.SetActive(true); // Show the quit option
            Time.timeScale = 0f; // Optional: pause the game
            //activate mouse cursor
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            quitMenuUI.SetActive(false); // Hide the quit option
            Time.timeScale = 1f; // Resume the game
            //deactivate mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}