using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneCode;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int coins;
    private double deathTime;
    private bool death = false;
    private bool end = false;
    private string song, extension;

    private AutoJumpManager autoJumpManager;
    private DebugManager debugManager;
    private PowerUpsManager powerUpsManager;
    private LightManager lightManager;
    private ZoneType zoneType;

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
    public void setDeath(bool b) { death = b; }
    public bool getEnd() { return end; }
    public void setEnd(bool b) { end = b; }

    public double getDeathTime() { return deathTime; }
    public void setDeathTime(double t) { deathTime = t; }

    public void setSong(string s) { song = s; }
    public string getSong() { return song; }
    public void setExtension(string s) { extension = s; }
    public string getExtension() { return extension; }

    public void setAutoJumpManager(AutoJumpManager a) { autoJumpManager = a; }
    public void setDebugManager(DebugManager d) { debugManager = d; }
    public void setPowerUpsManager(PowerUpsManager p) { powerUpsManager = p; }

    public void setDrumsEffect(LightManager d) { lightManager = d; }

    public PowerUpsManager getPowerUpsManager() { return powerUpsManager; }

    //Debug mode
    public DebugManager getDebugManager() { return debugManager; }

    public void changeDebugMode() { debugManager.changeDebugMode(); }



    //AutoJump
    public AutoJumpManager getAutoJumpManager() { return autoJumpManager; }


    public void changeAutoJumpMode() { autoJumpManager.ChangeAutoJumpMode(); }

    public FMOD.Studio.EventInstance getMusicInstance()
    {
        SelectMusic music = GameObject.FindGameObjectWithTag("Player").GetComponent<SelectMusic>();
        if (!music) Debug.Log("Se está intentando llamar a la canción cuando no hay ni player");

        return music.getEventInstance();
    }

    public void changeZone(ZoneType type)
    {
        if (lightManager == null)
        {
            Debug.LogWarning("El controlador de luces (objeto con script DrumsEffect) es nulo");
            return;
        }
        zoneType = type;
        Color lightColor, backgroundColor;
        if (type == ZoneType.HIGH)
        {
            lightColor = Color.red;
            backgroundColor = new Color(0.8f, 0.39f, 0.39f, 0.3f);
        }
        else if (type == ZoneType.LOW)
        {
            lightColor = new Color(0.62f, 0, 1f);
            backgroundColor = new Color(1f, 0, 1f, 0.3f);
        }
        else
        {
            lightColor = Color.blue;
            backgroundColor = new Color(0, 0.64f, 1f, 0.3f);
        }
        lightManager.SetLightColor(lightColor, backgroundColor);
    }
}