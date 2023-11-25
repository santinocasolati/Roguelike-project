using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomCreator : MonoBehaviour
{
    private void OnEnable()
    {
        //Debug.Log("Creating Boss Room in: " + GameManager.instance.bossRoom.gameObject.name);

        GameManager.instance.levelLoaded.Invoke();
    }
}
