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
    public Action<bool> levelCompleted;
    public Map map;

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
        levelCompleted += LevelCompletedHandler;
    }

    void LevelLoadedHandler()
    {
        Debug.Log("Loaded");
    }

    void LevelCompletedHandler(bool win)
    {
        if (win)
        {
            Debug.Log("Win");
        } else
        {
            Debug.Log("Lose");
        }
    }
}
