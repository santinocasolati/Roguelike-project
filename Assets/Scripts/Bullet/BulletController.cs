using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int bulletDamage = 1;

    private float speed = 50f;
    private float timeToDestroy = 3f;

    public Vector3 target { get; set; }
    public bool hit { get; set; }

    private void OnEnable()
    {
        Destroy(gameObject, timeToDestroy);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (!hit && Vector3.Distance(transform.position, target) < .01f )
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        EnemyHP enemy = other.gameObject.GetComponent<EnemyHP>();

        if (enemy != null)
        {
            enemy.Damage(bulletDamage);
        }

        Destroy(gameObject);
    }
}
