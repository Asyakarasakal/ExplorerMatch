using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class TrayManager : MonoBehaviour
{
    public static TrayManager Instance;

    [Header("Tray")]
    public Transform slotContainer;
    public Transform matchAnimationLayer;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Undo System")]
    public Transform undoSpawnPoint;
    public GameObject undoPoofPrefab;

    private List<Transform> slots = new List<Transform>();

    private int currentMatchStartIndex;
    private bool isMatching = false;

    private class TrayData
    {
        public SelectableObject CurrentObject;
        public string ObjectID;
        public Sprite Icon;
    }

    private void Awake()
    {
        Instance = this;

        foreach (Transform slot in slotContainer)
        {
            slots.Add(slot);
        }
    }

    public Transform GetSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return null;

        return slots[index];
    }

    public int GetSlotCount()
    {
        return slots.Count;
    }

    public Transform GetFirstEmptySlot()
    {
        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            if (!traySlot.IsOccupied && !traySlot.IsReserved)
            {
                traySlot.IsReserved = true;
                return slot;
            }
        }

        return null;
    }

    public Transform GetFirstSlot()
    {
        return slotContainer.GetChild(0);
    }

    public Vector3 GetSlotWorldPosition(Transform slot)
    {
        RectTransform rect = slot.GetComponent<RectTransform>();

        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rect.position);

        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(
            new Vector3(
                screenPoint.x,
                screenPoint.y,
                5f));

        return worldPoint;
    }

    public void CheckForMatch()
    {
        if (isMatching)
            return;

        Dictionary<string, int> objectCounts = new Dictionary<string, int>();
        bool matchFound = false;

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            if (!traySlot.IsOccupied)
                continue;

            if (!objectCounts.ContainsKey(traySlot.ObjectID))
            {
                objectCounts.Add(traySlot.ObjectID, 1);
            }
            else
            {
                objectCounts[traySlot.ObjectID]++;
            }
        }

        foreach (var item in objectCounts)
        {
            if (item.Value >= 3)
            {
                matchFound = true;
                Debug.Log("MATCH");

                HapticManager.Instance?.Vibrate();

                RearrangeMatchedObjects(item.Key);

                StartCoroutine(MatchRoutine(item.Key));
                break;
            }
        }

        if (!matchFound)
        {
            CheckLoseCondition();
        }
    }

    private void ShiftSlotsLeft()
    {
        List<TrayData> occupiedData = new List<TrayData>();

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            if (!traySlot.IsOccupied)
                continue;

            TrayData data = new TrayData();

            data.CurrentObject = traySlot.CurrentObject;
            data.ObjectID = traySlot.ObjectID;
            data.Icon = traySlot.IconImage.sprite;

            occupiedData.Add(data);
        }

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            traySlot.IsReserved = false;
            traySlot.IsOccupied = false;
            traySlot.CurrentObject = null;
            traySlot.ObjectID = "";

            if (traySlot.IconImage != null)
            {
                traySlot.IconImage.sprite = null;
                traySlot.IconImage.enabled = false;
            }
        }

        for (int i = 0; i < occupiedData.Count; i++)
        {
            TraySlot traySlot = slots[i].GetComponent<TraySlot>();

            traySlot.IsOccupied = true;
            traySlot.CurrentObject = occupiedData[i].CurrentObject;
            traySlot.ObjectID = occupiedData[i].ObjectID;

            if (traySlot.IconImage != null)
            {
                traySlot.IconImage.sprite = occupiedData[i].Icon;
                traySlot.IconImage.enabled = true;
            }
        }
    }

    private void RearrangeMatchedObjects(string objectID)
    {
        List<TrayData> allData = new List<TrayData>();

        int firstMatchIndex = -1;

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            if (!traySlot.IsOccupied)
                continue;

            TrayData data = new TrayData();

            data.CurrentObject = traySlot.CurrentObject;
            data.ObjectID = traySlot.ObjectID;
            data.Icon = traySlot.IconImage.sprite;

            if (traySlot.ObjectID == objectID && firstMatchIndex == -1)
            {
                firstMatchIndex = allData.Count;
                currentMatchStartIndex = firstMatchIndex;
            }

            allData.Add(data);
        }

        List<TrayData> matched = new List<TrayData>();
        List<TrayData> others = new List<TrayData>();

        foreach (TrayData data in allData)
        {
            if (data.ObjectID == objectID)
                matched.Add(data);
            else
                others.Add(data);
        }

        List<TrayData> finalOrder = new List<TrayData>();

        int otherIndex = 0;

        for (int i = 0; i < allData.Count; i++)
        {
            if (i >= firstMatchIndex && i < firstMatchIndex + matched.Count)
            {
                finalOrder.Add(matched[i - firstMatchIndex]);
            }
            else
            {
                finalOrder.Add(others[otherIndex]);
                otherIndex++;
            }
        }

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            traySlot.IsOccupied = false;
            traySlot.CurrentObject = null;
            traySlot.ObjectID = "";

            if (traySlot.IconImage != null)
            {
                traySlot.IconImage.sprite = null;
                traySlot.IconImage.enabled = false;
            }
        }

        for (int i = 0; i < finalOrder.Count; i++)
        {
            TraySlot traySlot = slots[i].GetComponent<TraySlot>();

            traySlot.IsOccupied = true;
            traySlot.CurrentObject = finalOrder[i].CurrentObject;
            traySlot.ObjectID = finalOrder[i].ObjectID;

            if (traySlot.IconImage != null)
            {
                traySlot.IconImage.sprite = finalOrder[i].Icon;
                traySlot.IconImage.enabled = true;
            }
        }
    }

    private void PlayMatchAnimation(string objectID)
    {
        List<TraySlot> matchedSlots = new List<TraySlot>();

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            if (!traySlot.IsOccupied)
                continue;

            if (traySlot.ObjectID == objectID)
            {
                matchedSlots.Add(traySlot);

                if (matchedSlots.Count == 3)
                    break;
            }
        }

        if (matchedSlots.Count < 3) return;

        TraySlot leftSlot = matchedSlots[0];
        TraySlot centerSlot = matchedSlots[1];
        TraySlot rightSlot = matchedSlots[2];

        RectTransform leftIcon = leftSlot.IconImage.rectTransform;
        RectTransform centerIcon = centerSlot.IconImage.rectTransform;
        RectTransform rightIcon = rightSlot.IconImage.rectTransform;

        Image leftClone = Instantiate(leftSlot.IconImage, matchAnimationLayer);
        Image centerClone = Instantiate(centerSlot.IconImage, matchAnimationLayer);
        Image rightClone = Instantiate(rightSlot.IconImage, matchAnimationLayer);

        leftClone.rectTransform.position = leftIcon.position;
        centerClone.rectTransform.position = centerIcon.position;
        rightClone.rectTransform.position = rightIcon.position;

        leftClone.rectTransform.localScale = Vector3.one;
        centerClone.rectTransform.localScale = Vector3.one;
        rightClone.rectTransform.localScale = Vector3.one;

        leftSlot.IconImage.enabled = false;
        centerSlot.IconImage.enabled = false;
        rightSlot.IconImage.enabled = false;

        Vector3 targetPosition = centerIcon.position;

        // Orijinal Süre: 0.18f
        leftClone.rectTransform
          .DOMove(targetPosition, 0.18f)
          .SetEase(Ease.InOutQuad)
          .OnComplete(() =>
          {
              Destroy(leftClone.gameObject);
          });

        rightClone.rectTransform
            .DOMove(targetPosition, 0.18f)
            .SetEase(Ease.InOutQuad);

        Destroy(centerClone.gameObject, 0.20f);
        Destroy(rightClone.gameObject, 0.20f);
    }

    private IEnumerator MatchRoutine(string objectID)
    {
        isMatching = true;

        // Orijinal Beklemeler: 0.05f ve 0.18f
        yield return new WaitForSeconds(0.05f);

        PlayMatchAnimation(objectID);

        yield return new WaitForSeconds(0.18f);

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            if (!traySlot.IsOccupied)
                continue;

            if (traySlot.ObjectID != objectID)
                continue;

            traySlot.IsOccupied = false;
            traySlot.CurrentObject = null;
            traySlot.ObjectID = "";

            if (traySlot.IconImage != null)
            {
                traySlot.IconImage.sprite = null;
                traySlot.IconImage.enabled = false;
            }
        }

        yield return new WaitForSeconds(0.05f);

        ShiftSlotsLeft();

        isMatching = false;

        CheckForMatch();
    }

    public bool IsMatching()
    {
        return isMatching;
    }

    public void CheckLoseCondition()
    {
        bool isAnyObjectInTransit = false;

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();
            if (traySlot != null && traySlot.IsReserved && !traySlot.IsOccupied)
            {
                isAnyObjectInTransit = true;
                break;
            }
        }

        if (isAnyObjectInTransit)
            return;

        bool isAnySlotFree = false;

        foreach (Transform slot in slotContainer)
        {
            TraySlot traySlot = slot.GetComponent<TraySlot>();

            if (traySlot != null && !traySlot.IsOccupied && !traySlot.IsReserved)
            {
                isAnySlotFree = true;
                break;
            }
        }

        if (!isAnySlotFree && LoseManager.Instance != null)
        {
            LoseManager.Instance.LoseGame();
        }
    }

    public bool UndoLastObject()
    {
        if (isMatching) return false;

        TraySlot lastOccupiedSlot = null;
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            TraySlot slot = slots[i].GetComponent<TraySlot>();
            if (slot.IsOccupied)
            {
                lastOccupiedSlot = slot;
                break;
            }
        }

        if (lastOccupiedSlot == null)
        {
            Debug.Log("Undo: Yuvada geri alýnacak obje yok!");
            return false;
        }

        string restoredID = lastOccupiedSlot.ObjectID;

        lastOccupiedSlot.AnimateUndoIcon();

        lastOccupiedSlot.IsOccupied = false;
        lastOccupiedSlot.CurrentObject = null;
        lastOccupiedSlot.ObjectID = "";

        if (BoardManager.Instance != null)
        {
            SelectableObject prefab = BoardManager.Instance.GetPrefabByID(restoredID);

            if (prefab != null)
            {
                Vector3 spawnPos = undoSpawnPoint != null ? undoSpawnPoint.position : new Vector3(0, 5f, 0);

                if (undoPoofPrefab != null)
                {
                    GameObject poof = Instantiate(undoPoofPrefab, spawnPos, Quaternion.identity);
                    Destroy(poof, 1.5f);
                }

                SelectableObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);
                newObj.objectID = restoredID;

                Vector3 originalScale = newObj.transform.localScale;
                newObj.transform.localScale = Vector3.zero;
                newObj.transform.DOScale(originalScale, 0.35f).SetEase(Ease.OutBack);
            }
        }

        ShiftSlotsLeft();

        return true;
    }
}