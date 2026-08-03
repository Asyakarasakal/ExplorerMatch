using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [Header("Main Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject profilePanel;

    [Header("UI Text Displays")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI profileLevelText;

    [Header("Profile Name Input")]
    public TMP_InputField nameInputField;

    [Header("Lives Settings")]
    public int maxLives = 5;
    private int currentLives;

    [Header("Booster Settings")]
    public int defaultBoosterAmount = 3;

    [Header("Scene Settings")]
    public string gameSceneName = "GameScene";

    private CanvasGroup mainCanvasGroup;
    private bool isLoading = false;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (profilePanel != null) profilePanel.SetActive(false);

        if (mainPanel != null)
        {
            mainCanvasGroup = mainPanel.GetComponent<CanvasGroup>();
            if (mainCanvasGroup == null)
            {
                mainCanvasGroup = mainPanel.AddComponent<CanvasGroup>();
            }
        }

        LoadUserData();
        LoadAudioSettings();
        UpdateUI();
    }

    private void LoadAudioSettings()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicState(isMusicOn);
        }
    }

    private void LoadUserData()
    {
        currentLives = PlayerPrefs.GetInt("CurrentLives", maxLives);

        if (nameInputField != null)
        {
            string savedName = PlayerPrefs.GetString("PlayerName", "Oyuncu");
            nameInputField.text = savedName;
        }
    }

    public void SavePlayerName(string newName)
    {
        PlayerPrefs.SetString("PlayerName", newName);
        PlayerPrefs.Save();
    }

    public void UpdateUI()
    {
        int savedIndex = PlayerPrefs.GetInt("CurrentLevel", 0);
        int displayLevel = savedIndex + 1;

        if (levelText != null) levelText.text = "LEVEL " + displayLevel;
        if (profileLevelText != null) profileLevelText.text = "Seviye " + displayLevel;

        if (livesText != null) livesText.text = currentLives.ToString();
    }

    public void PlayGame()
    {
        if (isLoading) return;

        if (currentLives > 0)
        {
            Time.timeScale = 1f;
            StartCoroutine(LoadSceneAsyncRoutine());
        }
    }

    private IEnumerator LoadSceneAsyncRoutine()
    {
        isLoading = true;
        SetMainPanelInteraction(false);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ConsumeLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            PlayerPrefs.SetInt("CurrentLives", currentLives);
            PlayerPrefs.Save();
            UpdateUI();
        }
    }

    public void RefillLives()
    {
        currentLives = maxLives;
        PlayerPrefs.SetInt("CurrentLives", currentLives);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void RefillBoosters()
    {
        PlayerPrefs.SetInt("Booster_Hint", defaultBoosterAmount);
        PlayerPrefs.SetInt("Booster_Freeze", defaultBoosterAmount);
        PlayerPrefs.SetInt("Booster_Undo", defaultBoosterAmount);
        PlayerPrefs.SetInt("Booster_Magnet", defaultBoosterAmount);
        PlayerPrefs.Save();
    }

    public void ToggleMusic()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
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
        bool isSFXOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
        isSFXOn = !isSFXOn;
        PlayerPrefs.SetInt("SFXOn", isSFXOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleVibration()
    {
        bool isVibrationOn = PlayerPrefs.GetInt("VibrationOn", 1) == 1;
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("VibrationOn", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        SetMainPanelInteraction(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        SetMainPanelInteraction(true);
    }

    public void OpenProfile()
    {
        if (profilePanel != null) profilePanel.SetActive(true);
        SetMainPanelInteraction(false);
    }

    public void CloseProfile()
    {
        if (profilePanel != null) profilePanel.SetActive(false);
        SetMainPanelInteraction(true);
    }

    private void SetMainPanelInteraction(bool enable)
    {
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.blocksRaycasts = enable;
            mainCanvasGroup.interactable = enable;
        }
    }

    public void ResetPlayerName()
    {
        if (nameInputField != null)
        {
            nameInputField.text = "";
            PlayerPrefs.DeleteKey("PlayerName");
            PlayerPrefs.Save();
            nameInputField.ActivateInputField();
        }
    }
}