using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance;

    [Header("Goals")]
    public List<GoalData> goals = new List<GoalData>();

    [Header("UI")]
    public Transform goalPanel;
    public GameObject goalItemPrefab;

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

                break;
            }
        }
    }
}