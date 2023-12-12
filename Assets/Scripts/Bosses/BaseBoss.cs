using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyHP))]
public class BaseBoss : MonoBehaviour
{
    private void Start()
    {
        GetComponent<EnemyHP>().enemyKilled += BossDefeated;
    }

    public void BossDefeated()
    {
        GameManager.instance.levelCompleted.Invoke(true);
    }
}
