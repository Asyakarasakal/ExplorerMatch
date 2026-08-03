using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board")]
    public List<SelectableObject> boardObjects = new List<SelectableObject>();

    [Header("Prefabs for Undo")]
    public List<SelectableObject> allObjectPrefabs = new List<SelectableObject>();

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
    }

    public bool IsBoardEmpty()
    {
        return boardObjects.Count == 0;
    }

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