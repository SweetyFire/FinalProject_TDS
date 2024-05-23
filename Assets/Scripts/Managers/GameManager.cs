using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : InitializableBehavior
{
    public static GameManager Instance { get; private set; }
    public static bool GamePaused => Time.timeScale <= 0f;
    public bool IsGameOver { get; private set; }

    public UnityEvent OnGameOver;

    public override void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        if (Time.timeScale > 0f)
        {
            PauseGame();
        }
        else
        {
            ContinueGame();
        }
    }

    public void GameOver()
    {
        IsGameOver = true;
        OnGameOver?.Invoke();
    }
}
