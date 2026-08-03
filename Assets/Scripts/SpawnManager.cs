using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public LevelData currentLevel;

    [Header("Spawn Physical Settings")]
    [Tooltip("Objelerin spawn olacaðý taban yükseklik çarpaný")]
    public float spawnHeightOffset = 0.1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLevel = LevelManager.Instance.currentLevel;

        SpawnLevelObjects();
    }

    private void SpawnLevelObjects()
    {
        int totalObjectCount = 0;

        foreach (LevelObject levelObject in currentLevel.levelObjects)
        {
            for (int i = 0; i < levelObject.count; i++)
            {
                totalObjectCount++;

                Vector3 basePosition = GetRandomSpawnPosition();

                float heightAddition = Random.Range(0.02f, 0.08f);
                Vector3 spawnPosition = basePosition + Vector3.up * (spawnHeightOffset + heightAddition);

                Quaternion randomRotation = Quaternion.Euler(
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f)
                );

                GameObject spawnedObject = Instantiate(levelObject.prefab, spawnPosition, randomRotation);

                Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.drag = 2f;
                    rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
                }
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 center = BoardManager.Instance.spawnArea.position;

        float width = BoardManager.Instance.spawnArea.localScale.x;
        float height = BoardManager.Instance.spawnArea.localScale.z;

        float randomX = Random.Range(center.x - (width * 0.4f), center.x + (width * 0.4f));
        float randomZ = Random.Range(center.z - (height * 0.4f), center.z + (height * 0.4f));

        return new Vector3(randomX, center.y, randomZ);
    }
}