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

    // --- YENÝ EKLENEN METOTLAR ---

    /// <summary>
    /// LosePanel üzerindeki 'X' butonuna týklanýnca çaðrýlacak metot.
    /// </summary>
    public void OpenQuitConfirmFromLose()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(false); // LosePanel'i geçici olarak gizle
        }

        if (PauseManager.Instance != null)
        {
            // QuitConfirmPanel'e bizi LosePanel'in çaðýrdýðýný haber veriyoruz
            PauseManager.Instance.OpenQuitConfirmPanel("LosePanel");
        }
    }

    /// <summary>
    /// QuitConfirmPanel'deki 'X' (Geri) butonuna basýlýnca LosePanel'i tekrar açan metot.
    /// </summary>
    public void ReopenLosePanel()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }
}