using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Win
    }

    public GameState currentState = GameState.MainMenu;

    public static GameManager Instance { get; private set; }

    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject winUI;
    public GameObject playingUI;
    public GameObject mainMenuUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SetState(GameState.MainMenu);
        
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }
    
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void Start()
    {
        SetState(GameState.MainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing) PauseGame();
            else if (currentState == GameState.Paused) ResumeGame();
        }
    }
    
    private void HandleClientConnected(ulong clientId)
    {
        if (!IsServer && clientId == NetworkManager.Singleton.LocalClientId)
        {
            SetState(GameState.Playing);
        }
    }
    
    public void SetState(GameState newState)
    {
        currentState = newState;
        Time.timeScale = (newState == GameState.Playing) ? 1f : 0f;

        mainMenuUI?.SetActive(newState == GameState.MainMenu);
        playingUI?.SetActive(newState == GameState.Playing);
        pauseMenuUI?.SetActive(newState == GameState.Paused);
        gameOverUI?.SetActive(newState == GameState.GameOver);
        winUI?.SetActive(newState == GameState.Win);

        Cursor.lockState = (newState == GameState.Playing)
            ? CursorLockMode.Locked
            : CursorLockMode.None;
        Cursor.visible = newState != GameState.Playing;
    }
    
    // networked start
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        OnClientConnected();
    }
    
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        OnClientConnected();
    }
    
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        SetState(GameState.Playing);
    }
    
    private void OnClientConnected()
    {
        SetState(GameState.Playing);
    }
    
    // pause sync
    public void PauseGame()
    {
        SetState(GameState.Paused);
        if (IsServer) PauseGameClientRpc();
        else PauseGameRequestServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void PauseGameRequestServerRpc() => PauseGameClientRpc();
    
    [ClientRpc]
    private void PauseGameClientRpc() => SetState(GameState.Paused);
    
    // resume sync
    public void ResumeGame()
    {
        SetState(GameState.Playing);
        if (IsServer) ResumeGameClientRpc();
        else ResumeGameRequestServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ResumeGameRequestServerRpc() => ResumeGameClientRpc();
    
    [ClientRpc]
    private void ResumeGameClientRpc() => SetState(GameState.Playing);
    
    // game over sync
    public void GameOver()
    {
        SetState(GameState.GameOver);
        if (IsServer) GameOverClientRpc();
    }
    
    [ClientRpc]
    private void GameOverClientRpc() => SetState(GameState.GameOver);
    
    // win sync
    public void WinGame()
    {
        SetState(GameState.Win);
        if (IsServer) WinGameClientRpc();
    }
    
    [ClientRpc]
    private void WinGameClientRpc() => SetState(GameState.Win);
    
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}