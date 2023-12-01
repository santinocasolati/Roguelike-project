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

    private void OnEnable()
    {
        Invoke(nameof(GenerateEnemies), timeToSpawn);
    }

    void GenerateEnemies()
    {
        Bounds roomBounds = gameObject.GetComponent<BoxCollider>().bounds;

        int enemiesAmount = Random.Range(minEnemies, maxEnemies);

        for (int i = 0; i < enemiesAmount; i++)
        {
            Vector3 randomPosition = GetRandomPosition(roomBounds);
            GameObject selectedPrefab = ChooseRandomPrefab();

            GameObject decoration = Instantiate(selectedPrefab, randomPosition, Quaternion.identity, transform);
            decoration.transform.position = new Vector3(decoration.transform.position.x, 0.01f, decoration.transform.position.z);

            decoration.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
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
                Random.Range(roomBounds.min.x, roomBounds.max.x),
                Random.Range(roomBounds.min.y, roomBounds.max.y),
                Random.Range(roomBounds.min.z, roomBounds.max.z)
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

        int randomValue = Random.Range(0, totalPercentage);

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
