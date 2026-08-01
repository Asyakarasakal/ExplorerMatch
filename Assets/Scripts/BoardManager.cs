using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board")]
    public List<SelectableObject> boardObjects = new List<SelectableObject>();

    [Header("Prefabs for Undo")]
    public List<SelectableObject> allObjectPrefabs = new List<SelectableObject>(); // Inspector'dan tanýmlanacak prefab listesi

    public Transform spawnArea;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterObject(SelectableObject obj)
    {
        boardObjects.Add(obj);
    }

    public void RemoveObject(SelectableObject obj)
    {
        boardObjects.Remove(obj);
        Debug.Log("Board Object Count: " + boardObjects.Count);
    }

    public bool IsBoardEmpty()
    {
        return boardObjects.Count == 0;
    }

    /// <summary>
    /// ID'si verilen objenin Prefab'ýný listeden arar ve getirir.
    /// </summary>
    public SelectableObject GetPrefabByID(string id)
    {
        foreach (SelectableObject prefab in allObjectPrefabs)
        {
            if (prefab != null && prefab.objectID == id)
            {
                return prefab;
            }
        }
        return null;
    }
}