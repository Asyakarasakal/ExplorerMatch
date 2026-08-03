using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Levels")]
    public List<LevelData> levels = new List<LevelData>();

    [Header("Current Level")]
    public LevelData currentLevel;

    [HideInInspector]
    public int currentLevelIndex = 0;

    [Header("UI")]
    public TMP_Text levelText;

    [Header("Goal Card UI References")]
    public GameObject levelStartPanel;
    public Image goalCardImage;
    public Button playButton;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    private const string LEVEL_SAVE_KEY = "CurrentLevel";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSavedLevel();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndUpdateLevelUI();
        SetupGoalCard();
    }

    public void LoadSavedLevel()
    {
        if (levels == null || levels.Count == 0)
        {
            return;
        }

        currentLevelIndex = PlayerPrefs.GetInt(LEVEL_SAVE_KEY, 0);

        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }

        currentLevel = levels[currentLevelIndex];

        FindAndUpdateLevelUI();
    }

    public void SaveAndAdvanceNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        if (levels != null && nextLevelIndex >= levels.Count)
        {
            PlayerPrefs.SetInt(LEVEL_SAVE_KEY, 0);
            PlayerPrefs.Save();
            currentLevelIndex = 0;

            if (levels.Count > 0)
            {
                currentLevel = levels[0];
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        currentLevelIndex = nextLevelIndex;

        PlayerPrefs.SetInt(LEVEL_SAVE_KEY, currentLevelIndex);
        PlayerPrefs.Save();

        if (levels != null && levels.Count > 0)
        {
            currentLevel = levels[currentLevelIndex];
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void FindAndUpdateLevelUI()
    {
        if (levelText == null)
        {
            GameObject textObj = FindObjectInActiveScene("LevelText");
            if (textObj != null)
            {
                levelText = textObj.GetComponent<TMP_Text>();
            }
        }

        if (levelText != null)
        {
            levelText.text = "LEVEL " + (currentLevelIndex + 1);
        }
    }

    private void SetupGoalCard()
    {
        if (levelStartPanel == null)
        {
            levelStartPanel = FindObjectInActiveScene("LevelStartPanel");
        }

        if (levelStartPanel != null)
        {
            Image bgImage = levelStartPanel.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.raycastTarget = true;
            }

            if (goalCardImage == null)
            {
                Transform imgTrans = levelStartPanel.transform.Find("GoalCardImage");
                if (imgTrans != null) goalCardImage = imgTrans.GetComponent<Image>();
            }

            if (playButton == null)
            {
                Transform btnTrans = levelStartPanel.transform.Find("PlayButton");
                if (btnTrans != null) playButton = btnTrans.GetComponent<Button>();
            }

            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(StartLevelGame);
            }

            if (currentLevel != null && currentLevel.goalCardSprite != null && goalCardImage != null)
            {
                goalCardImage.sprite = currentLevel.goalCardSprite;
            }

            levelStartPanel.SetActive(true);

            if (TimerManager.Instance != null)
            {
                TimerManager.Instance.TogglePause(true);
            }
        }
    }

    public void StartLevelGame()
    {
        if (levelStartPanel != null)
        {
            levelStartPanel.SetActive(false);
        }

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.TogglePause(false);
        }
    }

    private GameObject FindObjectInActiveScene(string objectName)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = activeScene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in transforms)
            {
                if (t.gameObject.name == objectName)
                {
                    return t.gameObject;
                }
            }
        }
        return null;
    }

    [ContextMenu("Reset Level Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LEVEL_SAVE_KEY);
        PlayerPrefs.Save();
        currentLevelIndex = 0;
        if (levels != null && levels.Count > 0)
        {
            currentLevel = levels[0];
        }
        FindAndUpdateLevelUI();
    }
}