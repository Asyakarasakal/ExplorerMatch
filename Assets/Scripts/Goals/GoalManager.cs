using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance;

    [Header("Goals")]
    public List<GoalData> goals = new List<GoalData>();

    [Header("UI")]
    public Transform goalPanel;
    public GameObject goalItemPrefab;

    public GameObject winPanel;

    private List<GoalItemUI> goalItems = new List<GoalItemUI>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateGoalUI();
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

                    goalItems[goals.IndexOf(goal)]
                        .UpdateCount(previousCount, goal.currentCount);
                }

                if (goal.currentCount <= 0)
                {
                    goalItems[goals.IndexOf(goal)].HideGoal();
                }

                Debug.Log(goal.objectID + " : " + goal.currentCount);
                /*

                if (AreAllGoalsCompleted())
                {
                    Debug.Log("LEVEL COMPLETE!");

                    if (winPanel != null)
                    {
                        winPanel.SetActive(true);
                    }

                    Time.timeScale = 0f;
                } */

                if (AreAllGoalsCompleted())
                {
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


    private IEnumerator ShowWinPanel()
    {
        Debug.Log("LEVEL COMPLETE!");

        yield return new WaitForSeconds(1.5f);

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }


    public void NextLevel()
    {
        Time.timeScale = 1f;

        LevelManager.Instance.currentLevelIndex++;

        if (LevelManager.Instance.currentLevelIndex >= LevelManager.Instance.levels.Count)
        {
            LevelManager.Instance.currentLevelIndex = 0;
        }

        LevelManager.Instance.currentLevel =
            LevelManager.Instance.levels[LevelManager.Instance.currentLevelIndex];

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}