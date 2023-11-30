using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecorationPrefabData
{
    public GameObject prefab;
    [Range(0, 100)] public int spawnPercentage;
}

public class ProceduralDecoration : MonoBehaviour
{
    public DecorationPrefabData[] decorationPrefabs;
    private int numberOfDecorations;

    void OnEnable()
    {
        GenerateDecorations();
    }

    void GenerateDecorations()
    {
        Bounds roomBounds = CalculateRoomBounds();

        for (int i = 0; i < numberOfDecorations; i++)
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
        Collider[] colliders = Physics.OverlapSphere(position, 1f);

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

        foreach (DecorationPrefabData prefabData in decorationPrefabs)
        {
            totalPercentage += prefabData.spawnPercentage;
        }

        int randomValue = Random.Range(0, totalPercentage);

        foreach (DecorationPrefabData prefabData in decorationPrefabs)
        {
            if (randomValue < prefabData.spawnPercentage)
            {
                return prefabData.prefab;
            }

            randomValue -= prefabData.spawnPercentage;
        }

        return decorationPrefabs[0].prefab;
    }

    Bounds CalculateRoomBounds()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        Bounds bounds = new Bounds(transform.position, Vector3.zero);

        foreach (Collider collider in colliders)
        {
            bounds.Encapsulate(collider.bounds);
        }

        Vector2 size = new Vector2(bounds.size.x, bounds.size.z);
        Vector2 refSize = new Vector2(1f, 1f);

        Vector2 calc = size / refSize;
        numberOfDecorations = (int)Mathf.Floor(calc.x + calc.y)/5;

        return bounds;
    }
}
