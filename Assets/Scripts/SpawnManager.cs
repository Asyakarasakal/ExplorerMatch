using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class SpawnManager : MonoBehaviour

{

    public static SpawnManager Instance;



    public LevelData currentLevel;



    [Header("Spawn Physical Settings")]

    [Tooltip("Objelerin spawn olacaðý taban yükseklik çarpaný")]

    public float spawnHeightOffset = 0.2f;



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



                // Her objeyi üst üste çakýþmamasý için biraz farklý yüksekliklerden ve rastgele rotasyonla býrakýyoruz

                Vector3 basePosition = GetRandomSpawnPosition();



                // Obje sayýsý arttýkça kademeli olarak biraz daha yüksekten düþmelerini saðlýyoruz

                // Eskiden (0.1f, 0.5f) olan kýsmý daha küçük yapýyoruz

                float heightAddition = Random.Range(0.02f, 0.08f);

                Vector3 spawnPosition = basePosition + Vector3.up * (spawnHeightOffset + heightAddition);



                // Rastgele doðal bir açý veriyoruz (Match Factory görünümü için)

                Quaternion randomRotation = Quaternion.Euler(

                    Random.Range(0f, 360f),

                    Random.Range(0f, 360f),

                    Random.Range(0f, 360f)

                );



                GameObject spawnedObject = Instantiate(levelObject.prefab, spawnPosition, randomRotation);



                // Fizik motorunun objeyi doðal düþürmesi için Rigidbody kontrolü

                Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();

                if (rb != null)

                {

                    rb.isKinematic = false;

                    rb.useGravity = true;

                    rb.drag = 2f;

                    // Hafif rastgele kuvvet vererek daha doðal daðýlmalarýný saðlýyoruz

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



        // Kenarlara çok yapýþmamalarý için %80'lik iç alanda spawn ediyoruz

        float randomX = Random.Range(center.x - (width * 0.4f), center.x + (width * 0.4f));

        float randomZ = Random.Range(center.z - (height * 0.4f), center.z + (height * 0.4f));



        return new Vector3(randomX, center.y, randomZ);

    }

}