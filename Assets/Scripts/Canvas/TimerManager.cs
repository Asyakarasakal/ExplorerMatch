using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;

    public float timeRemaining = 60f;

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;

                Debug.Log("TIME UP!");

                LoseManager.Instance.LoseGame();
            }

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
