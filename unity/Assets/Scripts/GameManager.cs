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

    public void addCoin() { coins++; }
    public bool getDeath() { return death; }
    public void setDeath(bool b) { 
        death = b;
    }

}