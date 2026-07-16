using UnityEngine;
using System.Collections.Generic;

public class TrayManager : MonoBehaviour
{
    public static TrayManager Instance;

    [Header("Tray")]
    public Transform slotContainer;

    [Header("Camera")]
    public Camera mainCamera;

    private List<Transform> slots = new List<Transform>();

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
                Debug.Log(slot.name + " | IsOccupied = " + traySlot.IsOccupied);
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
            Debug.Log(item.Key + " = " + item.Value);

            if (item.Value >= 3)
            {
                Debug.Log("MATCH! " + item.Key);

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

                Debug.Log("Matched Slots: " + matchedSlots.Count);

                foreach (TraySlot slot in matchedSlots)
                {
                    Debug.Log("Matched: " + slot.name);

                }

                foreach (TraySlot slot in matchedSlots)
                {
                    slot.IsOccupied = false;
                    slot.ObjectID = "";
                }


            }
        }
    }

}