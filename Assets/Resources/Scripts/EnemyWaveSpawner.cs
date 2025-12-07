using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;       // assign your enemy prefab
    public int maxEnemiesPerWave = 3;    // up to 3 enemies
    public float waveDelay = 5f;         // delay before next wave

    [Header("Spawn Area")]
    public BoxCollider spawnArea;        // assign the square collider

    private  readonly List<GameObject> currentWaveEnemies = new();
    private  readonly bool spawning = false;

    void Start()
    {
        StartCoroutine(SpawnWaveLoop());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        { 
            SpawnWave(); 
        }
    }

    IEnumerator SpawnWaveLoop()
    {
        while (true)
        {
            // Wait until all enemies are dead
            yield return new WaitUntil(() => currentWaveEnemies.Count == 0);

            // Delay before next wave
            yield return new WaitForSeconds(waveDelay);

            // Spawn new wave
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        int enemyCount = Random.Range(1, maxEnemiesPerWave + 1);
        currentWaveEnemies.Clear();

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = GetRandomPointInArea();
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            currentWaveEnemies.Add(enemy);

            // Hook into enemy death
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                // When enemy dies, remove from list
                ai.OnEnemyDeath += () => currentWaveEnemies.Remove(enemy);
            }
        }
    }

    Vector3 GetRandomPointInArea()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.center.y;
        return new Vector3(x, y, z);
    }
}
