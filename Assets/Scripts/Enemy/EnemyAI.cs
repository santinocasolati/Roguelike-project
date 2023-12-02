using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float attackTime = 1.4f;
    public float attackDistance = 1.8f;

    private NavMeshAgent agent;
    private bool attacking = false;
    private float currentTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!attacking)
        {
            Transform playerObj = GameObject.Find("PlayerObj").transform;

            if (Vector3.Distance(transform.position, playerObj.position) < attackDistance)
            {
                attacking = true;
                currentTime = 0;
                agent.isStopped = true;
            } else
            {
                agent.isStopped = false;
                agent.SetDestination(playerObj.position);
            }
        } else
        {
            currentTime += Time.deltaTime;

            if (currentTime >= attackTime)
            {
                attacking = false;
            }
        }
    }
}
