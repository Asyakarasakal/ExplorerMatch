using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoseManager : MonoBehaviour
{
    public static LoseManager Instance;

    public GameObject losePanel;

    public bool IsGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    public void LoseGame()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.DisablePauseButton();
        }

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }

    public bool GameIsOver()
    {
        return IsGameOver;
    }

    public void Retry()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ConsumeLife();
        }

        Time.timeScale = 1f;

        int remainingLives = 0;
        if (PauseManager.Instance != null)
        {
            remainingLives = PauseManager.Instance.GetCurrentLives();
        }
        else
        {
            remainingLives = PlayerPrefs.GetInt("PlayerLives", 0);
        }

        if (remainingLives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OpenQuitConfirmFromLose()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.OpenQuitConfirmPanel("LosePanel");
        }
    }

    public void ReopenLosePanel()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }
}