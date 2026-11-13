using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    Playing,
    Paused,
    Victory,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración de Partida")]
    [SerializeField] private int scoreToWin = 50;
    [SerializeField] private PlayerHealth playerHealth;

    public GameState CurrentState { get; private set; }
    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
            playerHealth.OnPlayerDied += HandlePlayerDied;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;
            playerHealth.OnPlayerDied -= HandlePlayerDied;
        }
    }

    private void Start()
    {
        ChangeState(GameState.Start);
        StartGame(); // Arranca automáticamente en Playing; podés llamarlo desde un botón si hacés menú
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Pausar / Reanudar con Escape o P
        if (kb.escapeKey.wasPressedThisFrame || kb.pKey.wasPressedThisFrame)
        {
            if (CurrentState == GameState.Playing)
                PauseGame();
            else if (CurrentState == GameState.Paused)
                ResumeGame();
        }

        // Reiniciar rápido con R si terminó la partida
        if ((CurrentState == GameState.Victory || CurrentState == GameState.GameOver) && kb.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }
    }

    public void StartGame()
    {
        Score = 0;
        Time.timeScale = 1f;
        ChangeState(GameState.Playing);
        Debug.Log($"[GameManager] Puntaje inicial: {Score}/{scoreToWin}");
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        Time.timeScale = 0f;
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        Time.timeScale = 1f;
        ChangeState(GameState.Playing);
    }

    public void AddScore(int points)
    {
        if (CurrentState != GameState.Playing) return;

        Score += points;
        Debug.Log($"[GameManager] +{points} puntos. Puntaje actual: {Score}/{scoreToWin}");

        if (Score >= scoreToWin)
        {
            TriggerVictory();
        }
    }

    public void TriggerVictory()
    {
        if (CurrentState != GameState.Playing) return;
        Time.timeScale = 0f;
        ChangeState(GameState.Victory);
        Debug.Log("[GameManager] ¡VICTORIA! Presioná R para reiniciar.");
    }

    private void HandlePlayerHealthChanged(int current, int max)
    {
        Debug.Log($"[GameManager] Vida del jugador: {current}/{max}");
    }

    private void HandlePlayerDied()
    {
        if (CurrentState != GameState.Playing) return;
        Time.timeScale = 0f;
        ChangeState(GameState.GameOver);
        Debug.Log("[GameManager] GAME OVER. Presioná R para reiniciar.");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameManager] Cambio de estado -> {CurrentState}");
    }
}