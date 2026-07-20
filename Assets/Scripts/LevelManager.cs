using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
   public static LevelManager Instance;

    [Header("Current Level")]
    public LevelData currentLevel;

    private void Awake()
    {
       Instance = this;
    }
}
