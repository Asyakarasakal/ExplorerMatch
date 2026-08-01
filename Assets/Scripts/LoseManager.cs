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
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}