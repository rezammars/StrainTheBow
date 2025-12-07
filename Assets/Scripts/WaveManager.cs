using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float timeBetweenWaves = 5f;

    [Header("Spawn Timing")]
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 2f;
    public bool useRandomTiming = true;

    [Header("Spawn Area")]
    public float spawnX = 60f;
    public float minY = -25f;
    public float maxY = 25f;
    public bool useRandomY = true;

    [Header("Wave Settings")]
    public int[] enemiesPerWave = { 10, 15, 20 }; // Wave 1:10, 2:15, 3:20
    public float healthMultiplierPerWave = 1.2f;
    public float speedMultiplierPerWave = 1.1f;

    [Header("Movement Patterns")]
    public Enemy.MovementType wave1Movement = Enemy.MovementType.StraightLeft;
    public Enemy.MovementType wave2Movement = Enemy.MovementType.SineWave;
    public Enemy.MovementType wave3Movement = Enemy.MovementType.ZigZag;

    public int currentWave = 0;
    private int enemiesAlive = 0;
    private bool isSpawning = false;
    private bool allWavesCompleted = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Invoke("StartNextWave", 2f);
    }

    public void StartNextWave()
    {
        if (currentWave >= enemiesPerWave.Length)
        {
            allWavesCompleted = true;
            return;
        }

        if (isSpawning || allWavesCompleted) return;

        StartCoroutine(SpawnWave(currentWave));
    }

    public void StartWave(int waveNumber)
    {
        if (waveNumber <= 0 || waveNumber > enemiesPerWave.Length)
        {
            Debug.LogError($"❌ Invalid wave number: {waveNumber}");
            return;
        }

        currentWave = waveNumber;
        Debug.Log($"🎯 WaveManager: Manually starting Wave {currentWave}");
        StartNextWave();
    }

    IEnumerator SpawnWave(int waveNumber)
    {
        isSpawning = true;

        int enemiesThisWave = GetEnemiesForWave(waveNumber);
        enemiesAlive = enemiesThisWave;

        Enemy.MovementType currentWaveMovement = GetMovementTypeForWave(waveNumber);

        for (int i = 0; i < enemiesThisWave; i++)
        {
            SpawnEnemyAtRandomY(waveNumber, i, currentWaveMovement);
            yield return new WaitForSeconds(GetRandomSpawnDelay());
        }

        isSpawning = false;
    }

    void SpawnEnemyAtRandomY(int waveNumber, int enemyIndex, Enemy.MovementType movementType)
    {
        if (enemyPrefab == null) return;

        float spawnY = useRandomY ? Random.Range(minY, maxY) : 0f;
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Enemy enemyController = enemy.GetComponent<Enemy>();
        if (enemyController != null)
        {
            int health = Mathf.RoundToInt(5 * Mathf.Pow(healthMultiplierPerWave, waveNumber - 1));
            float speed = 2f * Mathf.Pow(speedMultiplierPerWave, waveNumber - 1);

            enemyController.SetStats(health, speed);
            enemyController.SetLanePosition(spawnY);
            enemyController.SetMovementType(movementType);
        }
    }

    int GetEnemiesForWave(int waveNumber)
    {
        return (waveNumber > 0 && waveNumber <= enemiesPerWave.Length) 
            ? enemiesPerWave[waveNumber - 1] 
            : 5;
    }

    float GetRandomSpawnDelay()
    {
        return useRandomTiming 
            ? Random.Range(minSpawnDelay, maxSpawnDelay) 
            : (minSpawnDelay + maxSpawnDelay) / 2f;
    }

    public void OnEnemyKilled()
    {
        if (isSpawning) return;

        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            if (currentWave >= enemiesPerWave.Length)
            {
                allWavesCompleted = true;
            }
            else
            {
                Invoke("StartNextWave", timeBetweenWaves);
            }
        }
    }

    Enemy.MovementType GetMovementTypeForWave(int waveNumber)
    {
        switch (waveNumber)
        {
            case 1: return wave1Movement;
            case 2: return wave2Movement;
            case 3: return wave3Movement;
            default: return Enemy.MovementType.StraightLeft;
        }
    }

    public bool IsSpawning() => isSpawning;
}