using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    void Start()
    {
        GetComponent<EnemyHP>().enemyKilled += EnemyKilledHandler;
    }

    void EnemyKilledHandler()
    {
        Destroy(gameObject);
    }
}
