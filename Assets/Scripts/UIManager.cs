using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI enemiesNumber, scoreNumber, timerNumber;
    [SerializeField]
    private float gameTime = 7f;
    private float gameTimer;
    [SerializeField]
    private float finalScore;
    [SerializeField]
    private float addedPoints;
    [SerializeField]
    private float timerPoints;
    [SerializeField]
    private string gameTimerCountdown;
    [SerializeField]
    private int currentEnemies;
    [SerializeField]
    private int totalEnemies;
    private bool isGameRunning = true;
    private bool spawningEnemies = true;
    private float spawningDelay = 0.1f;

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
        spawningEnemies = true;

        gameTimer = gameTime * 60f;
        timerPoints = (gameTimer * 10f);
        finalScore = 0f;
    }


    private void Update()
    {
        if (spawningEnemies)
        {
            SpawningDelay();
        }

        if (isGameRunning)
        {
            GameTimer();
        }
    }

    private void SpawningDelay()
    {
        spawningDelay -= Time.deltaTime;

        if (spawningDelay <= 0f)
        {
            spawningEnemies = false;
        }
    }

    public void ScoreManager(float points)
    {
        addedPoints += points;
        finalScore = addedPoints + timerPoints;
        finalScore = Mathf.Round((finalScore * 1000f) / 1000f);
        scoreNumber.text = finalScore.ToString();
    }

    public void EnemiesManager(int enemy)
    {
        currentEnemies += enemy;

        if (spawningEnemies)
        {
            totalEnemies = currentEnemies;
        }

        enemiesNumber.text = currentEnemies.ToString() + "/" + totalEnemies.ToString();
    }

    public void GameTimer()
    {
        gameTimer -= Time.deltaTime;
        timerPoints -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer % 60f);

        gameTimerCountdown = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerNumber.text = gameTimerCountdown;

        ScoreManager(0f);

        if (gameTimer <= 0f)
        {
            isGameRunning = false;
            Debug.Log("DEAD");
        }
    }
}
