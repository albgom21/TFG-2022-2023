using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int coins;
    private double deathTime;
    private bool death = false;
    private bool end = false;
    private bool powerUpQuality = false;
    private bool powerUpGravity = false;
    private string song, extension;

    private AutoJumpManager autoJumpManager;
    private DebugManager debugManager;

    void Awake()     // Comprobar que solo hay un GameManager.
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
        deathTime = 0;
    }

    public void addCoin() { coins++; }
    public bool getDeath() { return death; }
    public void setDeath(bool b) { death = b;}
    public bool getEnd() { return end; }
    public void setEnd(bool b) { end = b; }

    public double getDeathTime() { return deathTime; }
    public void setDeathTime(double t) { deathTime = t; }

    public bool getPowerUpGravity() { return powerUpGravity; }
    public void setPowerUpGravity(bool b) { powerUpGravity = b; }

    public bool getPowerUpQuality() { return powerUpQuality; }
    public void setPowerUpQuality(bool b) { powerUpQuality = b; }

    public void setSong(string s) { song = s; }
    public string getSong() { return song; }
    public void setExtension(string s) { extension = s; }
    public string getExtension() { return extension; }

    public void setAutoJumpManager(AutoJumpManager a) { autoJumpManager = a; }
    public void setDebugManager(DebugManager d) { debugManager = d; }



    //Debug mode
    public DebugManager getDebugManager() { return debugManager; }

    public void changeDebugMode()
    {
        debugManager.changeDebugMode();
    }

    //AutoJump
    public AutoJumpManager getAutoJumpManager() { return autoJumpManager; }


    public void changeAutoJumpMode()
    {
        autoJumpManager.changeAutoJumpMode();
    }
}