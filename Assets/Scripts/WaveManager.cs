using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float timeBeetwenWaves = 5f;

    [Header("Wave Settings")]
    public int baseEnemiesPerWave = 5;
    public float healthMultiplierPerWave = 1.2f;
    public float speedMultiplierPerWave = 1.1f;

    [Header("Spawn Formation")]
    public bool useLaneSystem = true;
    public float spawnDelay = 1f;

    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool isSpawning = false;

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
        if (isSpawning) return;

        currentWave++;
        StartCoroutine(SpawnWave(currentWave));
    }

    IEnumerator SpawnWave(int waveNumber)
    {
        isSpawning = true;

        int enemiesThisWave = CalculateEnemiesInWave(waveNumber);
        enemiesAlive = enemiesThisWave;

        for (int i = 0; i < enemiesThisWave; i++)
        {
            SpawnEnemyInLane(waveNumber, i);

            yield return new WaitForSeconds(spawnDelay);
        }
        isSpawning = false;
    }

    void SpawnEnemyInLane(int waveNumber, int enemyIndex)
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null) return;

        int laneIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[laneIndex];

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        Enemy enemyController = enemy.GetComponent<Enemy>();
        if (enemyController != null)
        {
            int health = Mathf.RoundToInt(5 * Mathf.Pow(healthMultiplierPerWave, waveNumber - 1));
            float speed = 2f * Mathf.Pow(speedMultiplierPerWave, waveNumber - 1);

            enemyController.SetStats(health, speed);

            enemyController.SetLanePosition(spawnPoint.position.y);
        }

        Debug.Log($"Spawned enemy in lane {laneIndex} at Y: {spawnPoint.position.y}");
    }

    int CalculateEnemiesInWave(int waveNumber)
    {
        return baseEnemiesPerWave + (waveNumber * 2);
    }

    public void EnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <=0 && !isSpawning)
        {
            Invoke("StartNextWave", timeBeetwenWaves);
        }
    }

    public void StartWave(int waveNumber, int enemyCount)
    {
        if (isSpawning) return;

        currentWave = waveNumber;
        StartCoroutine(SpawnWave(waveNumber));
    }
}