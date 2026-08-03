using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TraySlot : MonoBehaviour
{
    public bool IsOccupied = false;
    public SelectableObject CurrentObject;
    public string ObjectID;
    public Image IconImage;
    public RectTransform IconRect;

    public bool IsReserved = false;

    private void Awake()
    {
        IconImage = transform.Find("Icon").GetComponent<Image>();
        IconRect = IconImage.GetComponent<RectTransform>();
        IconImage.enabled = false;
    }

    public void AnimateUndoIcon()
    {
        if (IconImage == null || IconImage.sprite == null) return;

        GameObject tempIconObj = new GameObject("TempUndoIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        tempIconObj.transform.SetParent(IconImage.transform.parent, false);

        RectTransform tempRect = tempIconObj.GetComponent<RectTransform>();
        Image tempImage = tempIconObj.GetComponent<Image>();

        tempRect.position = IconImage.transform.position;
        tempRect.sizeDelta = IconImage.rectTransform.sizeDelta;
        tempImage.sprite = IconImage.sprite;
        tempImage.raycastTarget = false;

        IconImage.enabled = false;
        IconImage.sprite = null;

        Sequence undoSeq = DOTween.Sequence();
        undoSeq.Append(tempRect.DOScale(1.2f, 0.25f).SetEase(Ease.OutQuad));
        undoSeq.Join(tempRect.DOAnchorPosY(tempRect.anchoredPosition.y + 60f, 0.35f).SetEase(Ease.OutCubic));
        undoSeq.Join(tempImage.DOFade(0f, 0.35f).SetEase(Ease.InQuad));

        undoSeq.OnComplete(() =>
        {
            Destroy(tempIconObj);
        });
    }
}