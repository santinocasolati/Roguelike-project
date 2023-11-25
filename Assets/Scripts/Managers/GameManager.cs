using ooparts.dungen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Room bossRoom;
    public Action levelLoaded;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        levelLoaded += LevelLoadedHandler;
    }

    void LevelLoadedHandler()
    {
        Debug.Log("Loaded");
    }
}
