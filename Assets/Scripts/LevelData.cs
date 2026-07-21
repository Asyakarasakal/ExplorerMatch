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


[CreateAssetMenu(fileName = "New Level",
    menuName = "Explorer Match/Level Data")]


public class LevelData : ScriptableObject
{
    [Header("Level Settings")]
    public int levelNumber;
    public float levelTime = 60f;

    [Header("Level Objects")]
    public List<LevelObject> levelObjects = new List<LevelObject>();
}
