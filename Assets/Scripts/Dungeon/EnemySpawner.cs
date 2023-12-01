using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemiesData
{
    public GameObject prefab;
    [Range(0, 100)] public int spawnPercentage;
}

public class EnemySpawner : MonoBehaviour
{
    public int minEnemies = 5;
    public int maxEnemies = 15;
    public float timeToSpawn = 3f;

    public EnemiesData[] enemies;

    private int enemiesAlive;

    public Action roomCleared;

    private void OnEnable()
    {
        Invoke(nameof(GenerateEnemies), timeToSpawn);
    }

    void GenerateEnemies()
    {
        Bounds roomBounds = gameObject.GetComponent<BoxCollider>().bounds;

        int enemiesAmount = UnityEngine.Random.Range(minEnemies, maxEnemies);
        enemiesAlive = enemiesAmount;

        for (int i = 0; i < enemiesAmount; i++)
        {
            Vector3 randomPosition = GetRandomPosition(roomBounds);
            GameObject selectedPrefab = ChooseRandomPrefab();

            GameObject enemy = Instantiate(selectedPrefab, randomPosition, Quaternion.identity, transform);
            enemy.transform.SetPositionAndRotation(new Vector3(enemy.transform.position.x, 0.01f, enemy.transform.position.z), Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0));
            
            EnemyHP hp = enemy.GetComponent<EnemyHP>();

            if (hp != null)
            {
                hp.enemyKilled += RoomsEnemiesHandler;
            }
        }
    }

    void RoomsEnemiesHandler()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            roomCleared.Invoke();
        }
    }

    Vector3 GetRandomPosition(Bounds roomBounds)
    {
        Vector3 randomPosition = Vector3.zero;
        int attempts = 0;
        const int maxAttempts = 100;

        while (attempts < maxAttempts)
        {
            randomPosition = new Vector3(
                UnityEngine.Random.Range(roomBounds.min.x, roomBounds.max.x),
                UnityEngine.Random.Range(roomBounds.min.y, roomBounds.max.y),
                UnityEngine.Random.Range(roomBounds.min.z, roomBounds.max.z)
            );

            if (!IsPositionOverlapping(randomPosition))
            {
                return randomPosition;
            }

            attempts++;
        }

        Debug.LogWarning("Unable to find a valid position for decoration.");
        return Vector3.zero;
    }

    bool IsPositionOverlapping(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);

        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger && collider.transform != transform)
            {
                return true;
            }
        }

        return false;
    }

    GameObject ChooseRandomPrefab()
    {
        int totalPercentage = 0;

        foreach (EnemiesData prefabData in enemies)
        {
            totalPercentage += prefabData.spawnPercentage;
        }

        int randomValue = UnityEngine.Random.Range(0, totalPercentage);

        foreach (EnemiesData prefabData in enemies)
        {
            if (randomValue < prefabData.spawnPercentage)
            {
                return prefabData.prefab;
            }

            randomValue -= prefabData.spawnPercentage;
        }

        return enemies[0].prefab;
    }
}
