using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board")]
    public List<SelectableObject> boardObjects = new List<SelectableObject>();


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


}