using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("In-Game Pause Button")]
    public Button pauseButton;

    [Header("Main Panels")]
    public GameObject pausePanel;
    public GameObject quitConfirmPanel;

    [Header("End Game Panels")]
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("UI Displays (Opsiyonel / Oyun Ýçinde Silinebilir)")]
    public TextMeshProUGUI livesText; // Oyun içinde kullanýlmýyorsa Inspector'da boþ kalabilir

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    // Dynamic state variables
    private bool isMusicOn;
    private bool isSFXOn;
    private bool isVibrationOn;

    // YENÝ: Onay panelini açan önceki paneli hafýzada tutar ("PausePanel" veya "LosePanel")
    private string previousPanelName = "";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(false);

        LoadAudioSettings();
        UpdateLivesUI();
    }

    private void LoadAudioSettings()
    {
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        isSFXOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
        isVibrationOn = PlayerPrefs.GetInt("VibrationOn", 1) == 1;

        AudioListener.pause = !isMusicOn;
    }

    public void UpdateLivesUI()
    {
        if (livesText != null)
        {
            int currentLives = PlayerPrefs.GetInt("CurrentLives", 5);
            livesText.text = currentLives.ToString();
        }
    }

    public void ConsumeLife()
    {
        int currentLives = PlayerPrefs.GetInt("CurrentLives", 5);
        if (currentLives > 0)
        {
            currentLives--;
            PlayerPrefs.SetInt("CurrentLives", currentLives);
            PlayerPrefs.Save();
            UpdateLivesUI();
            Debug.Log("Oyundan çýkýldý, 1 Can Düþtü! Kalan Can: " + currentLives);
        }
    }

    // YENÝ DÜZELTME: PlayerPrefs üzerindeki gerçek can sayýsýný döndürür
    public int GetCurrentLives()
    {
        return PlayerPrefs.GetInt("CurrentLives", 5);
    }

    public void DisablePauseButton()
    {
        if (pauseButton != null) pauseButton.interactable = false;
    }

    public void EnablePauseButton()
    {
        if (pauseButton != null) pauseButton.interactable = true;
    }

    public void OpenPausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            DisablePauseButton();
        }
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(false);

        Time.timeScale = 1f;
        EnablePauseButton();
    }

    // --- AKILLI QUIT CONFIRM PANEL METOTLARI ---

    // 1. Parametre alan metot (Hangi panelden gelindiðini kaydeder)
    public void OpenQuitConfirmPanel(string comingFrom)
    {
        previousPanelName = comingFrom;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(true);

        Time.timeScale = 0f;
        DisablePauseButton();
    }

    // 2. Parametresiz kullaným (PausePanel'deki 'X' veya 'Çýkýþ' butonundan týklandýðýnda varsayýlan PausePanel kabul eder)
    public void OpenQuitConfirmPanel()
    {
        OpenQuitConfirmPanel("PausePanel");
    }

    // 3. QuitConfirmPanel üzerindeki 'X' (Geri) butonuna basýldýðýnda çaðrýlýr
    public void BackToPausePanel()
    {
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(false);

        // Nereden geldiysek oraya geri dön!
        if (previousPanelName == "LosePanel")
        {
            if (LoseManager.Instance != null)
            {
                LoseManager.Instance.ReopenLosePanel();
            }
        }
        else // Varsayýlan olarak PausePanel'e dön
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }

        Time.timeScale = 0f;
        DisablePauseButton();
    }

    public void ConfirmQuitToMainMenu()
    {
        ConsumeLife();
        Time.timeScale = 1f;
        EnablePauseButton();

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        EnablePauseButton();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- TOGGLE SETTINGS (SENKRONÝZE) ---

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        AudioListener.pause = !isMusicOn;
        Debug.Log("In-Game Müzik Durumu: " + (isMusicOn ? "AÇIK" : "KAPALI"));
    }

    public void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        PlayerPrefs.SetInt("SFXOn", isSFXOn ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("In-Game Ses Efektleri: " + (isSFXOn ? "AÇIK" : "KAPALI"));
    }

    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("VibrationOn", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("In-Game Titreþim: " + (isVibrationOn ? "AÇIK" : "KAPALI"));
    }
}