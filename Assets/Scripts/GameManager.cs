using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int maxLives = 9;
    [SerializeField] private Transform RespawnPoint;
    [SerializeField] private GameObject pauseMenuUI;

    public GameState currentState;

    public enum GameState { Title, Playing, Paused, GameOver, Victory }

    public int Score { get; private set; } = 0;
    public int Lives { get; private set; } = 3;

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
        if (currentState == GameState.Playing && Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
        else if (currentState == GameState.Paused && Input.GetKeyDown(KeyCode.P))
        {
            ResumeGame();
        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        switch (newState)
        {
            case GameState.Title:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("TitleMenu");
                break;

            case GameState.Playing:
                Lives = 3;
                Score = 0;
                SceneManager.LoadScene("GameScene");
                StartCoroutine(AssignRespawnPointAfterSceneLoad());
                break;

            case GameState.GameOver:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("GameOverMenu");
                break;

            case GameState.Victory:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("VictoryMenu");
                break;
        }
    }

    private IEnumerator AssignRespawnPointAfterSceneLoad()
    {
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

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform pauseTransform = canvas.transform.Find("PauseMenu");
            if (pauseTransform != null)
            {
                pauseMenuUI = pauseTransform.gameObject;
                pauseMenuUI.SetActive(false);
                Debug.Log("PauseMenu UI assigned from Canvas");
            }
            else
            {
                Debug.LogWarning("PauseMenu not found under Canvas");
            }
        }
        else
        {
            Debug.LogError("Canvas not found in GameScene!");
        }
    }


    public void PlayerDied()
    {
        Lives--;

        if (Lives > 0)
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
        Lives = Mathf.Clamp(Lives + amount, 0, maxLives);
        Debug.Log($"Life updated: {Lives}");
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2f); // small delay after death

        var player = Object.FindFirstObjectByType<PlayerMovement>();
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

    public void PauseGame()
    {
        Time.timeScale = 0f;
        currentState = GameState.Paused;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        Debug.Log("Game paused");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        currentState = GameState.Playing;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        Debug.Log("Game resumed");
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
