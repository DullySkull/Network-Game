using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Playing, Paused, GameOver }
    public GameState currentState = GameState.Playing;

    public GameObject pauseMenuUI;
    public GameObject gameOverUI;

    void Start()
    {
        SetState(GameState.Playing);
    }

    void Update()
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

    public void SetState(GameState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                if (pauseMenuUI != null)
                    pauseMenuUI.SetActive(false);
                if (gameOverUI != null)
                    gameOverUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                if (pauseMenuUI != null)
                    pauseMenuUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                if (gameOverUI != null)
                    gameOverUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
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
    }
}
