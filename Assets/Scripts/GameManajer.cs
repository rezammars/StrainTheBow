using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public int baseMaxLives = 3;
    public int totalWaves = 5;

    [Header("UI References")]
    public Text livesText;
    public Text waveText;
    public Text enemiesRemainingText;
    public Text scoreText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject waveStartPanel;
    public Text waveStartText;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 3f;

    private int currentBaseLives;
    private int currentWave = 0;
    private int enemiesInCurrentWave;
    private int enemiesDefeatedInWave;
    private int currentScore = 0;
    private bool isWaveActive = false;
    private bool gameEnded = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentBaseLives = baseMaxLives;
        currentScore = 0;
        UpdateUI();
        StartCoroutine(StartWaveSequence());
    }

    IEnumerator StartWaveSequence()
    {
        while (currentWave < totalWaves && !gameEnded)
        {
            currentWave++;
            StartNewWave(currentWave);

            if (waveStartPanel != null)
            {
                waveStartText.text = "WAVE " + currentWave;
                waveStartPanel.SetActive(true);
                yield return new WaitForSeconds(2f);
                waveStartPanel.SetActive(false);
            }

            yield return new WaitForSeconds(1f);

            isWaveActive = true;
            WaveManager.instance.StartWave(currentWave, enemiesInCurrentWave);

            yield return new WaitUntil(() => !isWaveActive || gameEnded);

            if (!gameEnded && currentWave < totalWaves)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        if (!gameEnded && currentWave >= totalWaves)
        {
            WinGame();
        }
    }

    void StartNewWave(int waveNumber)
    {
        enemiesInCurrentWave = CalculateEnemiesInWave(waveNumber);
        enemiesDefeatedInWave = 0;
        isWaveActive = true;
        UpdateUI();

        Debug.Log($"Starting Wave {waveNumber} with {enemiesInCurrentWave} enemies");
    }

    int CalculateEnemiesInWave(int wave)
    {
        return 5 + (wave * 2);
    }

    public void EnemyReachedBase()
    {
        if (gameEnded) return;

        currentBaseLives--;
        UpdateUI();

        if (currentBaseLives <= 0)
        {
            GameOver();
        }
    }

    public void EnemyDefeated()
    {
        if (gameEnded) return;

        enemiesDefeatedInWave++;
        AddScore(10);
        UpdateUI();

        if (enemiesDefeatedInWave >= enemiesInCurrentWave)
        {
            WaveCompleted();
        }
    }

    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        UpdateUI();
    }

    void WaveCompleted()
    {
        isWaveActive = false;

        if (currentWave >= totalWaves)
        {
            WinGame();
        }
        else
        {
            Debug.Log($"Wave {currentWave} completed! Preparing for next wave...");
        }
    }

    void GameOver()
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);
        Debug.Log("Game Over - 3 enemy reached your base!");
        Time.timeScale = 0f;
    }

    void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        winPanel.SetActive(true);
        Debug.Log("You Win! All waves completed!");
        Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        livesText.text = "Base Lives: " + currentBaseLives + "/" + baseMaxLives;
        waveText.text = "Wave: " + currentWave + "/" + totalWaves;
        scoreText.text = "Score: " + currentScore;

        if (isWaveActive)
        {
            int remaining = enemiesInCurrentWave - enemiesDefeatedInWave;
            enemiesRemainingText.text = "Enemies: " + remaining;
        }
        else
        {
            enemiesRemainingText.text = "Prepare for next wave!";
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsWaveActive()
    {
        return isWaveActive;
    }
}