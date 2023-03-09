using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Vector3 startPosition;
    private int coins;
    private bool death = false;

    void Awake()     //  Comprobar que solo hay un GameManager.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);
    }
    private void Start()
    {
        coins = 0;
    }

    public void setStartPosition(Vector3 pos)
    {
        startPosition = pos;
    }

    public Vector3 getStartPosition()
    {
        return startPosition;
    }

    public void addCoin() { coins++; }
    public bool getDeath() { return death; }
    public void setDeath() { 
        death = true;
        Invoke("restart", 0.1f);
    }

    public void restart()
    {
        death = false;
    }
}