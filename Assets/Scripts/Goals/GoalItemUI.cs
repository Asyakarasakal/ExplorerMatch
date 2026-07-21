using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoalItemUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;

    public void Setup(GoalData goalData)
    {
        icon.sprite = goalData.icon;
        countText.text = goalData.currentCount.ToString();
    }

    public void UpdateCount(int startCount, int endCount)
    {
        countText.text = endCount.ToString();
    }

    public void HideGoal()
    {
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(0.7f);

        gameObject.SetActive(false);
    }
}