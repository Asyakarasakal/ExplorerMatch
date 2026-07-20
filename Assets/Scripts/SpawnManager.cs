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

        Debug.Log("Aktif Level: " + currentLevel.name);

        Debug.Log("Prefab Sayýsý:" + currentLevel.spawnPrefabs.Count);

        foreach( GameObject prefab in currentLevel.spawnPrefabs )
        {
            Debug.Log(prefab.name);
        }

        Vector3 randomPosition = GetRandomSpawnPosition();

        Debug.Log("Spawn Position: " + randomPosition);


        for (int i = 0; i < currentLevel.spawnCount; i++)
        {
            int randomIndex = Random.Range(0, currentLevel.spawnPrefabs.Count);

            GameObject randomPrefab = currentLevel.spawnPrefabs[randomIndex];

            Vector3 targetPosition = GetRandomSpawnPosition();

            Vector3 spawnPosition = targetPosition + Vector3.up * 0.75f;

            GameObject spawnedObject = Instantiate(randomPrefab, spawnPosition, Quaternion.identity);

            spawnedObject.transform
               .DOMove(targetPosition, 0.25f)
               .SetDelay(i * 0.03f)
               .SetEase(Ease.OutQuad);
        };
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
