using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int hp;

    public Action enemyKilled;

    private int maxHp;

    private void Awake()
    {
        maxHp = hp;
    }

    public void Damage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            enemyKilled.Invoke();
        }
    }

    public void Heal(int heal)
    {
        hp += heal;

        if (hp > maxHp)
        {
            hp = maxHp;
        }
    }
}
