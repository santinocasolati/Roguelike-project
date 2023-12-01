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
            EnemySpawner enemySpawner = gameObject.GetComponent<EnemySpawner>();
            enemySpawner.enabled = true;
            enemySpawner.roomCleared += RoomClearedHandler; 
        } else
        {
            Destroy(gameObject.GetComponent<EnemySpawner>());
        }
    }

    private void RoomClearedHandler()
    {
        Invoke(nameof(OpenDoors), 3f);
    }

    private void CloseDoors()
    {
        foreach (Transform door in transform.Find("Doors"))
        {
            door.gameObject.SetActive(true);
        }
    }

    private void OpenDoors()
    {
        foreach (Transform door in transform.Find("Doors"))
        {
            door.gameObject.SetActive(false);
        }
    }
}
