using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoseManager : MonoBehaviour
{
    public static LoseManager Instance;

    public GameObject losePanel;

    public bool IsGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    public void LoseGame()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        Debug.Log("GAME LOST!");


        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public bool GameIsOver()
    {
        return IsGameOver;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}