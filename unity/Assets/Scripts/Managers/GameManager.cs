using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZoneCode;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private double deathTime;
    private bool death = false;
    private int lastBeatBeforeDeath;
    private bool end = false;
    private string song, extension;

    private AutoJumpManager autoJumpManager;
    private DebugManager debugManager;
    private PowerUpsManager powerUpsManager;
    private LightManager lightManager;
    private ReadTxt featureManager;

    void Awake()
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
        deathTime = lastBeatBeforeDeath = 0;
    }

    public bool GetDeath() { return death; }
    public void SetDeath(bool b) { death = b; }
    public bool GetEnd() { return end; }
    public void SetEnd(bool b) { end = b; }

    public double GetDeathTime() { return deathTime; }

    public int GetLastBeatBeforeDeath() { return lastBeatBeforeDeath; }
    public void SetDeathTime(double t) {
        deathTime = t;
        UpdateLastBeatBeforeDeath();
    }

    private void UpdateLastBeatBeforeDeath()
    {
        List<float> beats = featureManager.GetBeatsInTime();

        float deathTimeWithDelay = (float) deathTime - Constants.DELAY_TIME;

        lastBeatBeforeDeath = 0;
        while (lastBeatBeforeDeath < beats.Count && deathTimeWithDelay > beats[lastBeatBeforeDeath]) lastBeatBeforeDeath++;

    }

    public void SetSong(string s) { song = s; }
    public string GetSong() { return song; }
    public void SetExtension(string s) { extension = s; }
    public string GetExtension() { return extension; }

    // Manager Setters
    public void SetAutoJumpManager(AutoJumpManager a) { autoJumpManager = a; }
    public void SetDebugManager(DebugManager d) { debugManager = d; }
    public void SetPowerUpsManager(PowerUpsManager p) { powerUpsManager = p; }
    public void SetLightManager(LightManager l) { lightManager = l; }
    public void SetFeatureManager(ReadTxt r) { featureManager = r; }

    public PowerUpsManager GetPowerUpsManager() { return powerUpsManager; }

    public LightManager GetLightManager() { return lightManager; }

    //Debug mode
    public DebugManager GetDebugManager() { return debugManager; }

    public void ChangeDebugMode() { debugManager.changeDebugMode(); }

    public ReadTxt GetFeatureManager() { return featureManager; }


    //AutoJump
    public AutoJumpManager GetAutoJumpManager() { return autoJumpManager; }


    public void ChangeAutoJumpMode() { autoJumpManager.ChangeAutoJumpMode(); }

    public FMOD.Studio.EventInstance GetMusicInstance()
    {
        MainMusic music = GameObject.FindGameObjectWithTag("Player").GetComponent<MainMusic>();
        if (!music) Debug.Log("Se está intentando llamar a la canción cuando no hay ni player");

        return music.getEventInstance();
    }

    public void ChangeZone(ZoneType type)
    {
        if (lightManager == null)
        {
            Debug.LogError("Light Manager is null");
            return;
        }
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