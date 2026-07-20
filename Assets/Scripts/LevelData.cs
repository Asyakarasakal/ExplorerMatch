using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level",
    menuName = "Explorer Match/Level Data")]


public class LevelData : ScriptableObject
{
    [Header("Spawn Settings")]
    public List<GameObject> spawnPrefabs = new List<GameObject>();

    public int spawnCount = 10;
}
