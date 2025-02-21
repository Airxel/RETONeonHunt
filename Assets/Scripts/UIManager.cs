using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private float gameTime = 7f;
    private float gameTimer;
    [SerializeField]
    private float finalScore;
    [SerializeField]
    private string gameTimerCountdown;
    private bool isGameRunning = true;

    //Singleton
    public static UIManager instance;

    private void Awake()
    {
        if (UIManager.instance == null)
        {
            UIManager.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        isGameRunning = true;

        gameTimer = gameTime * 60f;
        finalScore = 0f;
    }


    private void Update()
    {
        if (isGameRunning)
        {
            GameTimer();
        }
    }

    public void ScoreManager(float points)
    {
        finalScore = (gameTimer * 10f) + points;
        finalScore = Mathf.Round((finalScore * 1000f) / 1000f);
    }

    public void GameTimer()
    {
        gameTimer -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer % 60f);

        gameTimerCountdown = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (gameTimer <= 0)
        {
            isGameRunning = false;
            Debug.Log("DEAD");
        }
    }
}
