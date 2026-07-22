using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Levels")]
    public List<LevelData> levels = new List<LevelData>();

    [Header("Current Level")]
    public LevelData currentLevel;

    [HideInInspector]
    public int currentLevelIndex = 0;

    private void Awake()
    {
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.Save();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 0);

        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }

        currentLevel = levels[currentLevelIndex];

        Debug.Log("Loaded Level Index => " + currentLevelIndex);


        Debug.Log("Current Level => " + currentLevel.name);
    }
}
