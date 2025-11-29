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
    public int Version = 1;
    public string Savedat = "";
    public bool HasCheckpoint = false;
    public Vector3 LastCheckpointPosition { get; set; }


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

    public void SetState(GameState newState, bool isNewGame = true)
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
                if (isNewGame)
                {
                    Lives = 3;
                    Score = 0;
                }
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
        if (player != null)
        {
            if (HasCheckpoint)
                {
                Debug.Log("Respawning player at checkpoint.");
                }
            else if (RespawnPoint != null)
            {
                Debug.Log("Respawning player at default respawn point.");
                player.transform.position = RespawnPoint.position;
            }

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

    public void SavePrototypeState()
    {
        SaveData data = new SaveData();
        data.Player.Lives = Lives;
        data.Score = Score;

        var player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            Vector3 pos = player.transform.position;
            data.Player.Position = new float[] { pos.x, pos.y, pos.z };
            data.Player.Rotation = new QuaternionData(player.transform.rotation);

            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
                data.Player.CurrentHealth = health.GetCurrentHealth();

            var weaponController = player.GetComponent<PlayerWeaponController>();
            if (weaponController != null && weaponController.CurrentWeaponPrefab != null)
            {
                data.Player.EquippedWeaponId = "Weapons/" + weaponController.CurrentWeaponPrefab.name;
                data.Player.IsTwoHanded = weaponController.IsTwoHanded;
            }
        }

        if (RespawnPoint != null)
        {
            var p = RespawnPoint.position;
            data.Checkpoint.checkpointPosition = new float[] { p.x, p.y, p.z };
            data.Checkpoint.checkpointId = "RespawnPoint";
        }

        SaveSystem.Save(data);
    }

    public void NewGame()
    {
        SaveSystem.DeleteSave();
        SetState(GameState.Playing);
    }

    public void SetCheckpoint(Vector3 position)
    {
        HasCheckpoint = true;
        LastCheckpointPosition = position;
        SavePrototypeState();
    }

    public void ContinueGame()
    {
        var data = SaveSystem.Load();
        if (data == null)
        {
            Debug.LogWarning("No save found, starting new game.");
            SetState(GameState.Playing);
            return;
        }

        Lives = data.Player.Lives;
        Score = data.Score;

        HasCheckpoint = true;

        SetState(GameState.Playing, false);
        StartCoroutine(ApplyCheckpointAfterSceneLoad(data));
    }

    private IEnumerator ApplyCheckpointAfterSceneLoad(SaveData data)
    {
        PlayerMovement player = null;
        while (player == null)
        {
            yield return null;
            player = Object.FindFirstObjectByType<PlayerMovement>();
        }

        if (HasCheckpoint && data.Checkpoint.checkpointPosition?.Length == 3)
        {
            player.transform.position = new Vector3(
                data.Checkpoint.checkpointPosition[0],
                data.Checkpoint.checkpointPosition[1],
                data.Checkpoint.checkpointPosition[2]
            );
            player.transform.rotation = data.Player.Rotation.ToQuaternion();
            Debug.Log("Player respawned at checkpoint: ");
        }

            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
                health.SetHealth(data.Player.CurrentHealth);

            var weaponController = player.GetComponent<PlayerWeaponController>();
            if (weaponController != null && !string.IsNullOrEmpty(data.Player.EquippedWeaponId))
            {
                var weaponPrefab = Resources.Load<GameObject>(data.Player.EquippedWeaponId);
                if (weaponPrefab != null)
                {
                    weaponController.EquipWeapon(weaponPrefab, data.Player.IsTwoHanded);
                }
                
            }
        }

    }



