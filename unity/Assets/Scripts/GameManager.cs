using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Vector3 startPosition;
    private int coins;
    private bool death = false;
    private string song;

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
    public void setDeath(bool b) { death = b;}
    public void setSong(string s) { song = s; }
    public string getSong() { return song; }

}