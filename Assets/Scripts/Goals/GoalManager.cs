using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance;

    [Header("Goals")]
    public List<GoalData> goals = new List<GoalData>();

    [Header("UI")]
    public Transform goalPanel;
    public GameObject goalItemPrefab;
    public GameObject winPanel;

    [Header("Audio")]
    public AudioClip goalCompleteSound;

    private List<GoalItemUI> goalItems = new List<GoalItemUI>();

    public bool IsGameWon { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
        {
            goals = LevelManager.Instance.currentLevel.goals;

            foreach (GoalData goal in goals)
            {
                goal.currentCount = goal.requiredCount;
            }

            CreateGoalUI();
        }
    }

    private void CreateGoalUI()
    {
        foreach (GoalData goal in goals)
        {
            GameObject goalObject = Instantiate(goalItemPrefab, goalPanel);
            GoalItemUI goalItemUI = goalObject.GetComponent<GoalItemUI>();
            goalItemUI.Setup(goal);
            goalItems.Add(goalItemUI);
        }
    }

    public void CollectGoal(string objectID)
    {
        foreach (GoalData goal in goals)
        {
            if (goal.objectID == objectID)
            {
                int previousCount = goal.currentCount;

                if (goal.currentCount > 0)
                {
                    goal.currentCount--;
                    goalItems[goals.IndexOf(goal)].UpdateCount(previousCount, goal.currentCount);
                }

                if (goal.currentCount <= 0)
                {
                    goalItems[goals.IndexOf(goal)].HideGoal();
                }

                if (AreAllGoalsCompleted() && !IsGameWon)
                {
                    IsGameWon = true;
                    StartCoroutine(ShowWinPanel());
                }

                break;
            }
        }
    }

    public bool AreAllGoalsCompleted()
    {
        foreach (GoalData goal in goals)
        {
            if (goal.currentCount > 0)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsGoalIncomplete(string objectID)
    {
        foreach (GoalData goal in goals)
        {
            if (goal.objectID == objectID && goal.currentCount > 0)
            {
                return true;
            }
        }
        return false;
    }

    public List<string> GetActiveGoalIDs()
    {
        List<string> activeIDs = new List<string>();
        foreach (GoalData goal in goals)
        {
            if (goal.currentCount > 0)
            {
                activeIDs.Add(goal.objectID);
            }
        }
        return activeIDs;
    }

    private IEnumerator ShowWinPanel()
    {
        DOTween.KillAll();

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.DisablePauseButton();
        }

        yield return new WaitForSeconds(0.5f);

        if (winPanel != null)
        {
            Transform outroTrans = winPanel.transform.Find("OutroImage");

            if (outroTrans != null)
            {
                Image outroImage = outroTrans.GetComponent<Image>();

                if (outroImage != null && LevelManager.Instance != null && LevelManager.Instance.currentLevel != null)
                {
                    Sprite outroSprite = LevelManager.Instance.currentLevel.levelOutroSprite;
                    if (outroSprite != null)
                    {
                        outroImage.sprite = outroSprite;
                        outroImage.enabled = true;
                    }
                }
            }

            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        DOTween.KillAll();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SaveAndAdvanceNextLevel();
        }
    }
}