using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class TrayManager : MonoBehaviour
{
    public static TrayManager Instance;

    [Header("Tray")]
    public Transform slotContainer;

    [Header("Camera")]
    public Camera mainCamera;

    private List<Transform> slots = new List<Transform>();

    private int currentMatchStartIndex;


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

            if (!traySlot.IsOccupied)
            {
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
        Dictionary<string, int> objectCounts = new Dictionary<string, int>();

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
                Debug.Log("MATCH");

                HapticManager.Instance?.Vibrate();

                List<TraySlot> matchedSlots = new List<TraySlot>();

                foreach (Transform slot in slotContainer)
                {
                    TraySlot traySlot = slot.GetComponent<TraySlot>();

                    if (traySlot.IsOccupied && traySlot.ObjectID == item.Key)
                    {
                        matchedSlots.Add(traySlot);
                    }
                }

                RearrangeMatchedObjects(item.Key);

                StartCoroutine(MatchRoutine(item.Key));
            }
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

            traySlot.IsOccupied = false;
            traySlot.CurrentObject = null;
            traySlot.ObjectID = "";

            traySlot.IconImage.sprite = null;
            traySlot.IconImage.enabled = false;
        }

        for (int i = 0; i < occupiedData.Count; i++)
        {
            TraySlot traySlot = slots[i].GetComponent<TraySlot>();

            traySlot.IsOccupied = true;
            traySlot.CurrentObject = occupiedData[i].CurrentObject;
            traySlot.ObjectID = occupiedData[i].ObjectID;

            traySlot.IconImage.sprite = occupiedData[i].Icon;
            traySlot.IconImage.enabled = true;
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

            traySlot.IconImage.sprite = null;
            traySlot.IconImage.enabled = false;
        }

        for (int i = 0; i < finalOrder.Count; i++)
        {
            TraySlot traySlot = slots[i].GetComponent<TraySlot>();

            traySlot.IsOccupied = true;
            traySlot.CurrentObject = finalOrder[i].CurrentObject;
            traySlot.ObjectID = finalOrder[i].ObjectID;

            traySlot.IconImage.sprite = finalOrder[i].Icon;
            traySlot.IconImage.enabled = true;
        }
    }

    private void PlayMatchAnimation(List<TraySlot> matchedSlots)
    {
        RectTransform center = matchedSlots[1].IconImage.rectTransform;
        RectTransform left = matchedSlots[0].IconImage.rectTransform;
        RectTransform right = matchedSlots[2].IconImage.rectTransform;

        Sequence sequence = DOTween.Sequence();

        sequence.Join(left.DOAnchorPos(center.anchoredPosition, 0.25f));
        sequence.Join(right.DOAnchorPos(center.anchoredPosition, 0.25f));

        sequence.OnComplete(() =>
        {
            foreach (TraySlot traySlot in matchedSlots)
            {
                traySlot.IsOccupied = false;
                traySlot.CurrentObject = null;
                traySlot.ObjectID = "";

                traySlot.IconImage.sprite = null;
                traySlot.IconImage.enabled = false;

                traySlot.IconImage.rectTransform.anchoredPosition = Vector2.zero;
            }

            ShiftSlotsLeft();
        });

        sequence.Play();
    }

    private IEnumerator MatchRoutine(string objectID)
    {
        TestMergeAnimation();
        yield return new WaitForSeconds(0.55f);

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

            traySlot.IconImage.sprite = null;
            traySlot.IconImage.enabled = false;
        }

        ShiftSlotsLeft();
    }

    private void TestIconAnimation(TraySlot traySlot)
    {
        traySlot.IconRect.DOAnchorPosX(20f, 0.15f)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void TestMergeAnimation()
    {
        Debug.Log("TestMergeAnimation Çalýþtý");
        Debug.Log(currentMatchStartIndex);

        TraySlot left = slots[currentMatchStartIndex].GetComponent<TraySlot>();
        TraySlot middle = slots[currentMatchStartIndex + 1].GetComponent<TraySlot>();
        TraySlot right = slots[currentMatchStartIndex + 2].GetComponent<TraySlot>();

        Debug.Log(left.IconRect.anchoredPosition.x);
        Debug.Log(middle.IconRect.anchoredPosition.x);
        Debug.Log(right.IconRect.anchoredPosition.x);

        float middleX = middle.IconRect.anchoredPosition.x;

        left.IconRect.DOAnchorPosX(middleX, 0.2f);
        right.IconRect.DOAnchorPosX(middleX, 0.2f);
    }
}