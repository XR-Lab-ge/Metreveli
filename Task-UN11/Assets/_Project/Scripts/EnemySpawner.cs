using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public string name = "Wave";
    public GameObject gruntPrefab;
    public int gruntCount = 5;
    public GameObject bossPrefab;
    public int bossCount = 0;
    public float spawnRadius = 30f;
    public float spawnInterval = 0.5f;
}

public class EnemySpawner : MonoBehaviour
{
    public List<Wave> waves = new List<Wave>();
    public float delayBetweenWaves = 5f;
    public Transform[] spawnPoints;

    public System.Action OnAllWavesCompleted;
    public System.Action<int, int> OnWaveStarted;

    int currentWaveIndex = -1;
    List<GameObject> liveEnemies = new List<GameObject>();

    public int GetTotalEnemyCount()
    {
        int total = 0;
        foreach (var w in waves) total += w.gruntCount + w.bossCount;
        return total;
    }

    public void SpawnEnemies()
    {
        currentWaveIndex = -1;
        liveEnemies.Clear();
        StopAllCoroutines();
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        while (currentWaveIndex < waves.Count - 1)
        {
            currentWaveIndex++;
            OnWaveStarted?.Invoke(currentWaveIndex + 1, waves.Count);
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            yield return new WaitUntil(() => CountAlive() == 0);
            if (currentWaveIndex < waves.Count - 1)
                yield return new WaitForSeconds(delayBetweenWaves);
        }
        OnAllWavesCompleted?.Invoke();
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.gruntCount; i++)
        {
            SpawnOne(wave.gruntPrefab, wave.spawnRadius);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        for (int i = 0; i < wave.bossCount; i++)
        {
            SpawnOne(wave.bossPrefab, wave.spawnRadius);
            yield return new WaitForSeconds(wave.spawnInterval * 2f);
        }
    }

    void SpawnOne(GameObject prefab, float radius)
    {
        if (!prefab) return;
        Vector3 pos;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            Vector2 rand = Random.insideUnitCircle.normalized * Random.Range(15f, radius);
            pos = new Vector3(rand.x, 0.5f, rand.y);
        }
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        liveEnemies.Add(go);
    }

    int CountAlive()
    {
        liveEnemies.RemoveAll(g => g == null);
        return liveEnemies.Count;
    }
}