using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SaveSystem.DeleteSave(); // Clear previous save data when starting a new game
        GameManager.Instance.StartGame();
    }

    public void ContinueGame()
    {
        if (SaveSystem.SaveExists())
        {
            GameManager.Instance.ContinueGame();
        }
        else
        {
            Debug.Log("No save file found ? Starting a new game.");
            StartGame();
        }
    }

    public void ExitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the editor
#endif
    }

    public void LoadTitleMenu()
        {
            GameManager.Instance.LoadTitleMenu();
        }
    

    public void ResumeGame()
        {
            GameManager.Instance.ResumeGame();
        }

}
