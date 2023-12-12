using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float timeToSpawn = 3f;

    private void OnEnable()
    {
        Invoke(nameof(GenerateBoss), timeToSpawn);
    }

    void GenerateBoss()
    {
        GameObject boss = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        boss.transform.localPosition = new Vector3(0, 0.1f, 0);
    }
}
