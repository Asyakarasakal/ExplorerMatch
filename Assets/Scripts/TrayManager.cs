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

    public Transform matchAnimationLayer;

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

        leftClone.rectTransform
            .DOMove(targetPosition, 0.3f)
            .SetEase(Ease.InOutQuad);

        rightClone.rectTransform
            .DOMove(targetPosition, 0.3f)
            .SetEase(Ease.InOutQuad);


        Destroy(leftClone.gameObject, 0.35f);
        Destroy(centerClone.gameObject, 0.35f);
        Destroy(rightClone.gameObject, 0.35f);

    }

    private IEnumerator MatchRoutine(string objectID)
    {
        yield return new WaitForSeconds(0.2f);

        PlayMatchAnimation(objectID);

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

        yield return new WaitForSeconds(0.2f);

        ShiftSlotsLeft();
    }

   

    
}