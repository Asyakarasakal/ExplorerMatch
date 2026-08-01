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

    public bool IsReserved = false; // Seçilen obje ayný slota uçuyordu o yüzden ekledim.

    private void Awake()
    {
        IconImage = transform.Find("Icon").GetComponent<Image>();
        IconRect = IconImage.GetComponent<RectTransform>();
        IconImage.enabled = false;
    }

    /// <summary>
    /// Undo kullanýldýðýnda ikonun yukarý süzülüp kaybolma animasyonu (Pop & Fly Up)
    /// </summary>
    public void AnimateUndoIcon()
    {
        if (IconImage == null || IconImage.sprite == null) return;

        // Ýkonun görsel kopyasýný (Phantom) oluþturuyoruz
        GameObject tempIconObj = new GameObject("TempUndoIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        tempIconObj.transform.SetParent(IconImage.transform.parent, false);

        RectTransform tempRect = tempIconObj.GetComponent<RectTransform>();
        Image tempImage = tempIconObj.GetComponent<Image>();

        // Pozisyon, boyut ve sprite verilerini birebir kopyala
        tempRect.position = IconImage.transform.position;
        tempRect.sizeDelta = IconImage.rectTransform.sizeDelta;
        tempImage.sprite = IconImage.sprite;
        tempImage.raycastTarget = false;

        // Orijinal ikonu anýnda temizle
        IconImage.enabled = false;
        IconImage.sprite = null;

        // DOTween Animasyonu: Büyü (1.2x), Yukarý Süzül ve Þeffaflaþ (Fade Out)
        Sequence undoSeq = DOTween.Sequence();
        undoSeq.Append(tempRect.DOScale(1.2f, 0.25f).SetEase(Ease.OutQuad));
        undoSeq.Join(tempRect.DOAnchorPosY(tempRect.anchoredPosition.y + 60f, 0.35f).SetEase(Ease.OutCubic));
        undoSeq.Join(tempImage.DOFade(0f, 0.35f).SetEase(Ease.InQuad));

        // Animasyon bitince geçici objeyi temizle
        undoSeq.OnComplete(() =>
        {
            Destroy(tempIconObj);
        });
    }
}