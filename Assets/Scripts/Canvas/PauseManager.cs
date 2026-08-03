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

    [Header("UI Displays")]
    public TextMeshProUGUI livesText;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    private bool isMusicOn;
    private bool isSFXOn;
    private bool isVibrationOn;

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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicState(isMusicOn);
        }
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
        }
    }

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

    public void OpenQuitConfirmPanel(string comingFrom)
    {
        previousPanelName = comingFrom;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(true);

        Time.timeScale = 0f;
        DisablePauseButton();
    }

    public void OpenQuitConfirmPanel()
    {
        OpenQuitConfirmPanel("PausePanel");
    }

    public void BackToPausePanel()
    {
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(false);

        if (previousPanelName == "LosePanel")
        {
            if (LoseManager.Instance != null)
            {
                LoseManager.Instance.ReopenLosePanel();
            }
        }
        else
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

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicState(isMusicOn);
        }
    }

    public void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        PlayerPrefs.SetInt("SFXOn", isSFXOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("VibrationOn", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}