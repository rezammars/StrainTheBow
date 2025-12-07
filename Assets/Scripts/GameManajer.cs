using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public int baseMaxLives = 3;
    public int totalWaves = 3;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 3f; // ⭐ TAMBAH INI

    [Header("UI References")]
    public ScoreDisplay scoreDisplay;
    public WaveDisplay waveDisplay;
    public HeartsDisplay heartsDisplay;

    [Header("Game State")]
    public int currentScore = 0;
    public int currentBaseLives { get; private set; }
    public int currentWave { get; private set; }
    public int enemiesInCurrentWave { get; private set; }
    public int enemiesDefeatedInWave { get; private set; }

    [Header("Game Over UI")]
    public GameOverUI gameOverUI;

    private bool isWaveActive = false;
    private bool gameEnded = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentBaseLives = baseMaxLives;
        currentScore = 0;
        currentWave = 0;

        if (PlayerPrefs.GetInt("ShouldContinue", 0) == 1)
        {
            LoadContinueProgress();
        }

        UpdateAllUI();
        StartCoroutine(StartWaveSequence());
    }

    void LoadContinueProgress()
    {
        currentWave = PlayerPrefs.GetInt("CurrentWave", 0);
        currentScore = PlayerPrefs.GetInt("PlayerScore", 0);
        currentBaseLives = PlayerPrefs.GetInt("BaseLives", 3);

        PlayerPrefs.SetInt("ShouldContinue", 0);
        PlayerPrefs.Save();

        Debug.Log($"🎮 Continued - Wave: {currentWave}, Score: {currentScore}, Lives: {currentBaseLives}");
    }

    IEnumerator StartWaveSequence()
    {
        while (currentWave < totalWaves && !gameEnded)
        {
            currentWave++;
            StartNewWave(currentWave);

            // ⭐ TUNGGU SPAWNING SELESAI
            yield return WaitForWaveSpawningComplete();
            
            // ⭐ TUNGGU WAVE SELESAI (SEMUA ENEMY MATI/SAMPAI BASE)
            yield return WaitForWaveCompletion();

            // ⭐ TUNGGU ANTARA WAVE
            if (!gameEnded && currentWave < totalWaves)
            {
                Debug.Log($"⏰ Waiting {timeBetweenWaves}s for Wave {currentWave + 1}...");
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
        // Set jumlah enemy
        enemiesInCurrentWave = GetEnemiesForWave(waveNumber);
        enemiesDefeatedInWave = 0;
        isWaveActive = true;

        Debug.Log($"🚀 GameManager: Starting Wave {waveNumber}");

        // ⭐ TELL WAVEMANAGER TO START THIS WAVE
        if (WaveManager.instance != null)
        {
            WaveManager.instance.currentWave = waveNumber;
            WaveManager.instance.StartNextWave();
        }
        else
        {
            Debug.LogError("❌ WaveManager.instance is null!");
        }

        // Show wave display
        if (waveDisplay != null)
        {
            waveDisplay.ShowWave(waveNumber);
        }

        UpdateAllUI();
    }

    // ⭐ METHOD: TUNGGU SPAWNING SELESAI
    IEnumerator WaitForWaveSpawningComplete()
    {
        Debug.Log("⏳ Waiting for wave spawning to complete...");
        
        if (WaveManager.instance != null)
        {
            // Tunggu sampai WaveManager selesai spawning
            while (WaveManager.instance.IsSpawning())
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        Debug.Log("✅ Wave spawning completed!");
    }

    // ⭐ METHOD: TUNGGU WAVE SELESAI
    IEnumerator WaitForWaveCompletion()
    {
        Debug.Log($"⏳ Waiting for Wave {currentWave} completion...");
        
        // Tunggu sampai semua enemy dihitung
        while (enemiesDefeatedInWave < enemiesInCurrentWave)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log($"✅ Wave {currentWave} completed!");
    }

    int GetEnemiesForWave(int wave)
    {
        // Wave 1: 10, Wave 2: 15, Wave 3: 20
        switch (wave)
        {
            case 1: return 10;
            case 2: return 15;
            case 3: return 20;
            default: return 5;
        }
    }

    // ⭐ ENEMY MATI DITEMBAK - DAPAT SCORE
    public void EnemyKilledByPlayer()
    {
        if (gameEnded) return;

        enemiesDefeatedInWave++;
        AddScore(10);

        Debug.Log($"🎯 Enemy killed: {enemiesDefeatedInWave}/{enemiesInCurrentWave}");
        UpdateAllUI();
        SaveGameProgress();

        // ⭐ CEK APAKAH WAVE SUDAH SELESAI
        CheckWaveCompletion();
    }

    // ⭐ ENEMY SAMPAI BASE - TIDAK DAPAT SCORE
    public void EnemyReachedBase()
    {
        if (gameEnded) return;

        enemiesDefeatedInWave++;
        currentBaseLives--;

        Debug.Log($"🏠 Enemy reached base: {enemiesDefeatedInWave}/{enemiesInCurrentWave}");

        // Update hearts
        if (heartsDisplay != null)
        {
            heartsDisplay.UpdateHeartsDisplay();
        }

        UpdateAllUI();
        SaveGameProgress();

        // ⭐ CEK APAKAH WAVE SUDAH SELESAI
        CheckWaveCompletion();

        if (currentBaseLives <= 0)
        {
            GameOver();
        }
    }

    // ⭐ METHOD: CEK WAVE COMPLETION
    void CheckWaveCompletion()
    {
        if (enemiesDefeatedInWave >= enemiesInCurrentWave)
        {
            // ⭐ TAPI CEK DULU APAKAH MASIH ADA ENEMY DI SCENE
            Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
            if (remainingEnemies.Length == 0)
            {
                Debug.Log($"✅ Wave {currentWave} truly completed!");
                isWaveActive = false;
            }
            else
            {
                Debug.Log($"⚠️ Counter complete but {remainingEnemies.Length} enemies still in scene");
            }
        }
    }

    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        Debug.Log($"💰 Score +{scoreToAdd} = {currentScore}");
    }

    void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        Debug.Log("💀 GAME OVER!");
        
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver(false); // false = game over (bukan win)
        }
        else
        {
            Debug.LogError("❌ gameOverUI reference is null!");
        }

        // Reset save data
        PlayerPrefs.DeleteKey("CurrentWave");
        PlayerPrefs.DeleteKey("PlayerScore");
        PlayerPrefs.DeleteKey("BaseLives");
        PlayerPrefs.Save();
    }

    void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        Debug.Log("🎉 YOU WIN!");

        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver(true); // true = win
        }
    }

    void UpdateAllUI()
    {
        if (scoreDisplay != null)
            scoreDisplay.UpdateScoreDisplay();

        if (waveDisplay != null)
            waveDisplay.UpdateWaveDisplay();

        if (heartsDisplay != null)
            heartsDisplay.UpdateHeartsDisplay();
    }

    void SaveGameProgress()
    {
        PlayerPrefs.SetInt("CurrentWave", currentWave);
        PlayerPrefs.SetInt("PlayerScore", currentScore);
        PlayerPrefs.SetInt("BaseLives", currentBaseLives);
        PlayerPrefs.Save();
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