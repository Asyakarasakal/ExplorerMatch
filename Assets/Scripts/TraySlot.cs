using UnityEngine;
using UnityEngine.UI;

public class TraySlot : MonoBehaviour
{
    public bool IsOccupied = false;

    public SelectableObject CurrentObject;

    public string ObjectID;

    public Image IconImage;

    public RectTransform IconRect;

    private void Awake()
    {
        IconImage = transform.Find("Icon").GetComponent<Image>();

        IconRect = IconImage.GetComponent<RectTransform>();

        IconImage.enabled = false;
    }
}