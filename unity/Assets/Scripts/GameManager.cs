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

    public void AddCoin() { coins++; }
    public bool GetDeath() { return death; }
    public void SetDeath(bool b) { death = b; }
    public bool GetEnd() { return end; }
    public void SetEnd(bool b) { end = b; }

    public double GetDeathTime() { return deathTime; }
    public void SetDeathTime(double t) { deathTime = t; }

    public void SetSong(string s) { song = s; }
    public string GetSong() { return song; }
    public void SetExtension(string s) { extension = s; }
    public string GetExtension() { return extension; }

    public void SetAutoJumpManager(AutoJumpManager a) { autoJumpManager = a; }
    public void SetDebugManager(DebugManager d) { debugManager = d; }
    public void SetPowerUpsManager(PowerUpsManager p) { powerUpsManager = p; }

    public void SetDrumsEffect(LightManager d) { lightManager = d; }

    public PowerUpsManager GetPowerUpsManager() { return powerUpsManager; }

    //Debug mode
    public DebugManager GetDebugManager() { return debugManager; }

    public void ChangeDebugMode() { debugManager.changeDebugMode(); }



    //AutoJump
    public AutoJumpManager GetAutoJumpManager() { return autoJumpManager; }


    public void ChangeAutoJumpMode() { autoJumpManager.ChangeAutoJumpMode(); }

    public FMOD.Studio.EventInstance GetMusicInstance()
    {
        SelectMusic music = GameObject.FindGameObjectWithTag("Player").GetComponent<SelectMusic>();
        if (!music) Debug.Log("Se está intentando llamar a la canción cuando no hay ni player");

        return music.getEventInstance();
    }

    public void ChangeZone(ZoneType type)
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