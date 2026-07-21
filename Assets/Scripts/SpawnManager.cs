using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnManager : MonoBehaviour
{
   public static SpawnManager Instance;

    public LevelData currentLevel;

    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        currentLevel = LevelManager.Instance.currentLevel;

        foreach (GoalData goal in GoalManager.Instance.goals)
        {
            Debug.Log(goal.objectID + " -> " + goal.requiredCount);
        }

        Debug.Log("Aktif Level: " + currentLevel.name);


        foreach (LevelObject levelObject in currentLevel.levelObjects)
         {
             for (int i = 0; i < levelObject.count; i++)
             {
                 Vector3 targetPosition = GetRandomSpawnPosition();

                 Vector3 spawnPosition = targetPosition + Vector3.up * 0.75f;

                 GameObject spawnedObject = Instantiate(levelObject.prefab, spawnPosition, Quaternion.identity);

                 spawnedObject.transform
                   .DOMove(targetPosition, 0.25f)
                   .SetEase(Ease.OutQuad); 
            }

        }
    } 

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 center = BoardManager.Instance.spawnArea.position;

        float width = BoardManager.Instance.spawnArea.localScale.x;

        float height = BoardManager.Instance.spawnArea.localScale.z;

        float randomX = Random.Range(center.x - width / 2f, center.x + width / 2f);

        float randomZ = Random.Range(center.z - height / 2f, center.z + height / 2f);

        return new Vector3(randomX, center.y, randomZ);

    }

   

}
