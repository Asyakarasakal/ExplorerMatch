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
        // 1. Can düþür
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ConsumeLife();
        }

        Time.timeScale = 1f;

        // 2. Caný PauseManager'dan doðrudan kontrol et
        int remainingLives = 0;
        if (PauseManager.Instance != null)
        {
            remainingLives = PauseManager.Instance.GetCurrentLives();
        }
        else
        {
            // Eðer Instance yoksa PlayerPrefs'ten yedek kontrol yap
            remainingLives = PlayerPrefs.GetInt("PlayerLives", 0);
        }

        Debug.Log("Retry basýldý. Kalan can: " + remainingLives);

        // 3. Karar aný: Can varsa baþtan baþlat, yoksa Ana Menüye at
        if (remainingLives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("Can bitti! Ana menüye gidiliyor...");
            SceneManager.LoadScene("MainMenu"); // Ana menü sahne adýndan emin olalým
        }
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