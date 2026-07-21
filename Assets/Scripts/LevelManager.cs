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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentLevel = levels[currentLevelIndex];
    }
}
