using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Enemies,
    Loot,
    Boss,
    Shop
}

public class EnterRoom : MonoBehaviour
{
    private bool used;

    public RoomType roomType { get; set; }

    private void OnEnable()
    {
        used = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!used && other.gameObject.name == "PlayerObj")
        {
            used = true;
            ActivateRoom();
        }
    }

    private void ActivateRoom()
    {
        CloseDoors();

        if (roomType == RoomType.Enemies)
        {
            gameObject.GetComponent<EnemySpawner>().enabled = true;
        } else
        {
            Destroy(gameObject.GetComponent<EnemySpawner>());
        }
    }

    private void CloseDoors()
    {
        foreach (Transform door in transform.Find("Doors"))
        {
            door.gameObject.SetActive(true);
        }
    }
}
