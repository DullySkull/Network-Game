using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState currentState = GameState.MainMenu;

    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject playingUI;
    public GameObject mainMenuUI;

    void Start()
    {
        SetState(GameState.MainMenu);
    }

    void Update()
    {
        if (GameState.MainMenu != currentState)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentState == GameState.Playing)
                {
                    PauseGame();
                }
                else if (currentState == GameState.Paused)
                {
                    ResumeGame();
                }
            }

        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f;
                if (mainMenuUI != null)
                    mainMenuUI.SetActive(true);
                if (pauseMenuUI != null)
                    pauseMenuUI.SetActive(false);
                if (gameOverUI != null)
                    gameOverUI.SetActive(false);
                if (playingUI != null)
                    playingUI.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                if (mainMenuUI != null)
                    mainMenuUI.SetActive(false);
                if (pauseMenuUI != null)
                    pauseMenuUI.SetActive(false);
                if (gameOverUI != null)
                    gameOverUI.SetActive(false);
                if (playingUI != null)
                    playingUI.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                if (pauseMenuUI != null)
                    pauseMenuUI.SetActive(true);
                if (playingUI != null)
                    playingUI.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                if (gameOverUI != null)
                    gameOverUI.SetActive(true);
                if (playingUI != null)
                    playingUI.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        SetState(GameState.Playing);
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        SetState(GameState.Playing);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        SetState(GameState.Playing);
    }

    public void PauseGame()
    {
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
    }

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
}
