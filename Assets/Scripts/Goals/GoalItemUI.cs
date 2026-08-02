using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class GoalItemUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(GoalData goalData)
    {
        icon.sprite = goalData.icon;
        countText.text = goalData.currentCount.ToString();

        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
    }

    public void UpdateCount(int startCount, int endCount)
    {
        countText.text = endCount.ToString();
    }

    public void HideGoal()
    {
        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        // --- GOAL TAMAMLANMA SESÝNÝ ÇAL ---
        if (AudioManager.Instance != null && GoalManager.Instance != null && GoalManager.Instance.goalCompleteSound != null)
        {
            AudioManager.Instance.PlaySFX(GoalManager.Instance.goalCompleteSound);
        }

        // Önce hafif büyüsün
        yield return transform
            .DOScale(1.1f, 0.12f)
            .SetEase(Ease.OutBack)
            .WaitForCompletion();

        // Sonra küçülerek ve þeffaflaþarak kaybolsun
        Sequence seq = DOTween.Sequence();

        seq.Join(transform.DOScale(0f, 0.2f).SetEase(Ease.InBack));
        seq.Join(canvasGroup.DOFade(0f, 0.2f));

        yield return seq.WaitForCompletion();

        gameObject.SetActive(false);
    }
}