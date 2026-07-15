using UnityEngine;

public class TrayManager : MonoBehaviour
{
    public static TrayManager Instance;

    [Header("Tray")]
    public Transform slotContainer;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetFirstSlot()
    {
        return slotContainer.GetChild(0);
    }
}