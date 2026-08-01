using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelObject
{
    public string objectID;
    public GameObject prefab;
    public Sprite icon;
    public int count;
    public bool isGoalObject;
}

[CreateAssetMenu(fileName = "New Level", menuName = "Explorer Match/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Settings")]
    public int levelNumber;
    public float levelTime = 60f;

    [Header("Goal Card (Görev Kartý Visual)")]
    public Sprite goalCardSprite; // Her level'ýn baþlangýç kartýnda görünecek özel görsel

    [Header("Level Outro Card (Seviye Bitiþ Görseli)")]
    public Sprite levelOutroSprite; // Her level'ýn Win Panelinde görünecek özel görsel!

    [Header("Level Objects")]
    public List<LevelObject> levelObjects = new List<LevelObject>();

    [Header("Goals")]
    public List<GoalData> goals = new List<GoalData>();
}