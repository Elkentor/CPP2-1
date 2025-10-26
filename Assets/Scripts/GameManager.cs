using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int maxLives = 9;
    [SerializeField] private Transform RespawnPoint;
    public GameState currentState;

    public enum GameState { Title, Playing, GameOver }

    public int Score { get; private set; } = 0;
    public int PlayerLives { get; private set; } = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetState(GameState.Title);
    }

    private void Update()
    {
        if (currentState == GameState.GameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            LoadTitleMenu();
        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.Title:
                SceneManager.LoadScene("TitleMenu");
                break;
            case GameState.Playing:
                PlayerLives = 3;
                Score = 0;
                SceneManager.LoadScene("GameScene");
                StartCoroutine(AssignRespawnPointAfterSceneLoad());
                break;
            case GameState.GameOver:
                SceneManager.LoadScene("GameOverMenu");
                break;
        }
    }

    private IEnumerator AssignRespawnPointAfterSceneLoad()
    {
        // Wait one frame for the new scene to load
        yield return null;

        GameObject found = GameObject.Find("RespawnPoint");
        if (found != null)
        {
            RespawnPoint = found.transform;
            Debug.Log("RespawnPoint assigned: " + RespawnPoint.position);
        }
        else
        {
            Debug.LogError("RespawnPoint not found in GameScene!");
        }
    }

    public void PlayerDied()
    {
        PlayerLives--;

        if (PlayerLives > 0)
        {
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            //transit to game over state
            SetState(GameState.GameOver);
        }
    }

    public void AddScore(int amount)
    {
        Score = Mathf.Max(0, Score + amount);
        Debug.Log($"Score updated: {Score}");
    }

    public void AddLife(int amount)
    {
        PlayerLives = Mathf.Clamp(PlayerLives + amount, 0, maxLives);
        Debug.Log($"Life updated: {PlayerLives}");
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2f); // small delay after death

        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null && RespawnPoint != null)
        {
            Debug.Log("Respawning player at: " + RespawnPoint.position);
            player.transform.position = RespawnPoint.position;
            player.ResetState();

            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ResetHealth(); // restore health
            }

        }
        else
        {
            Debug.LogError("Respawn failed — missing player or respawn point!");
        }
    }

    public void StartGame()
    {
        Debug.Log("startgamecall");
        SetState(GameState.Playing);
    }

    public void LoadTitleMenu()
    {
        SetState(GameState.Title);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the editor
#endif
    }
}
