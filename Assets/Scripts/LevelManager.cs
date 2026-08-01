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
    public GameObject levelStartPanel; // Görev Kartý Paneli
    public Image goalCardImage;         // Görev Görseli
    public Button playButton;           // Oyna Butonu

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; // Ana Menü sahne adý

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
        StartCoroutine(SetupGoalCardRoutine());
    }

    public void LoadSavedLevel()
    {
        if (levels == null || levels.Count == 0)
        {
            Debug.LogWarning("LevelManager: Levels listesi boþ!");
            return;
        }

        currentLevelIndex = PlayerPrefs.GetInt(LEVEL_SAVE_KEY, 0);

        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }

        currentLevel = levels[currentLevelIndex];

        FindAndUpdateLevelUI();

        Debug.Log("Loaded Level Index => " + currentLevelIndex);
    }

    public void SaveAndAdvanceNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        // EÐER SON LEVEL BÝTTÝYSE (Örn: 10. level tamamlandýysa)
        if (levels != null && nextLevelIndex >= levels.Count)
        {
            // Ýlerlemeyi bir sonraki oyunda 1. seviyeden baþlasýn diye sýfýrla
            PlayerPrefs.SetInt(LEVEL_SAVE_KEY, 0);
            PlayerPrefs.Save();
            currentLevelIndex = 0;

            if (levels.Count > 0)
            {
                currentLevel = levels[0];
            }

            Debug.Log("Tüm seviyeler tamamlandý! Ana Menü'ye dönülüyor...");

            // Zaman akýþýný normale alýp Ana Menü sahnesini yükle
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        // HAKEN SON LEVEL DEÐÝLSE: Sonraki level'a geç ve kaydet
        currentLevelIndex = nextLevelIndex;

        PlayerPrefs.SetInt(LEVEL_SAVE_KEY, currentLevelIndex);
        PlayerPrefs.Save();

        if (levels != null && levels.Count > 0)
        {
            currentLevel = levels[currentLevelIndex];
        }

        Debug.Log("Saved Next Level Index => " + currentLevelIndex);

        // Sahneyi yeniden yükle (Yeni level baþlasýn)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void FindAndUpdateLevelUI()
    {
        if (levelText == null)
        {
            GameObject textObj = FindObjectEvenIfInactive("LevelText");
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

    /// <summary>
    /// Görev Kartýný kurar, görselini atar, butona dinleyici ekler ve arkaya týklanmasýný engeller.
    /// </summary>
    private IEnumerator SetupGoalCardRoutine()
    {
        yield return null; // 1 frame bekle

        if (levelStartPanel == null)
        {
            levelStartPanel = FindObjectEvenIfInactive("LevelStartPanel");
        }

        if (levelStartPanel != null)
        {
            // Panel üzerindeki Image bileþeninin týk geçirmesini kesinleþtiriyoruz
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

            // Oyna Butonu týklamasýný LevelManager içinde dinliyoruz
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(StartLevelGame);
            }

            // O anki level'ýn görselini karta bas
            if (currentLevel != null && currentLevel.goalCardSprite != null && goalCardImage != null)
            {
                goalCardImage.sprite = currentLevel.goalCardSprite;
            }

            // Paneli aç ve oyunu/süreyi dondur
            levelStartPanel.SetActive(true);

            if (TimerManager.Instance != null)
            {
                TimerManager.Instance.TogglePause(true);
            }
        }
    }

    /// <summary>
    /// Görev Kartýndaki Oyna butonuna basýlýnca çalýþýr
    /// </summary>
    public void StartLevelGame()
    {
        if (levelStartPanel != null)
        {
            levelStartPanel.SetActive(false);
        }

        // Oyunu ve süreyi baþlat
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.TogglePause(false);
        }

        Debug.Log("LevelManager: Oyna butonuna basýldý, level baþladý!");
    }

    private GameObject FindObjectEvenIfInactive(string objectName)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.None && obj.name == objectName && obj.scene.isLoaded)
            {
                return obj;
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
        Debug.Log("Level ilerlemesi sýfýrlandý!");
    }
}