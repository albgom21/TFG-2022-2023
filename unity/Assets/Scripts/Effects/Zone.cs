using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] private Material highSky;
    [SerializeField] private Material lowSky;
    [SerializeField] private GameObject features;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color highColor;
    [SerializeField] private Color lowColor;

    private Color originalColor;

    private Material originalSkyBox;

    private List<int> beatsZones = new List<int>();

    private int beatIniHigh = -1,
                beatIniLow = -1,
                beatEndHigh = -1,
                beatEndLow = -1;

    private float timeCount = 0;
    private bool[] once = { true, true, true, true };

    private float timeIniZoneHigh = 0;
    private float timeIniZoneLow = 0;
    private float timeEndZoneHigh = 0;
    private float timeEndZoneLow = 0;

    void Start()
    {
        originalSkyBox = RenderSettings.skybox;

        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        List<float> rmse = features.GetComponent<ReadTxt>().getRMSE();

        HighZone(beats, rmse);
        LowZone(beats, rmse);

        originalColor = playerSprite.color;

        //Debug.Log("Ini BEAT high: " + beatIniHigh);
        //Debug.Log("Fin BEAT high: " + beatEndHigh);

        //Debug.Log("Ini BEAT low: " + beatIniLow);
        //Debug.Log("Fin BEAT low: " + beatEndLow);

    }

    private void HighZone(List<float> beats, List<float> rmse)
    {
        float iniZone = -1,
             contIni = -1,
             balance = 0;

        int bestLength = -1;
        int actualLength = 0;
        int iniCont = 0;

        bool find = true;
        for (int i = 0; i < rmse.Count(); i++)
        {
            // Si está dentro del umbral y la zona no se ha iniciado guardar comienzo
            if (find && rmse[i] >= 0.925f)
            {
                contIni = i;
                iniZone = beats[i];
                iniCont = i;
                find = false;
                balance += rmse[i];
            }
            // Si ya se está en la zona y el balance cumple seguir agrandando la zona
            else if (iniZone >= 0 && balance >= ((i - contIni) * 0.85f) && rmse[i] >= 0.7f)
            {
                //Debug.Log("Rmse: " + rmse[i] + " B: " + balance + " BU: " + ((i - contIni) * 0.85f));
                if ((balance - ((i - contIni) * 0.85f)) >= 2)
                    balance -= 1.75f;

                actualLength++;
                balance += rmse[i];

                if (actualLength > bestLength)
                {
                    bestLength = actualLength;
                    timeIniZoneHigh = iniZone;
                    timeEndZoneHigh = beats[i];
                    beatEndHigh = i;
                    beatIniHigh = iniCont;
                }
            }
            else
            {
                actualLength = 0;
                balance = 0;
                find = true;
            }
        }

        beatsZones.Add(beatIniHigh);
        beatsZones.Add(beatEndHigh);

        //Invoke("iniZoneHighMethod", timeIniZoneHigh);
        //Invoke("endZoneMethod", timeEndZoneHigh);
        //Debug.Log("Ini zona high: " + timeIniZoneHigh);
        //Debug.Log("Fin zona high: " + timeEndZoneHigh);

    }

    private void LowZone(List<float> beats, List<float> rmse)
    {
        float bestIniZone = -1,
              iniZone = -1,
              contIni = -1,
              finZone = -1,
              balance = 0;

        int bestLength = -1;
        int actualLength = 0;
        int iniCont = 0;


        bool find = true;
        for (int i = 0; i < rmse.Count(); i++)
        {
            // Si está dentro del umbral y la zona no se ha iniciado guardar comienzo
            if (find && (rmse[i] >= 0.4f && rmse[i] < 0.5f))
            {
                //Debug.Log("NUEVA BUSQUEDA en: " + i +" de " + rmse.Count());
                contIni = i;
                iniZone = beats[i];
                iniCont = i;
                find = false;
                balance += rmse[i];

            }
            // Si ya se está en la zona y el balance cumple seguir agrandando la zona
            else if (iniZone >= 0 && balance <= ((i - contIni) * 0.3f) && (((i - contIni) * 0.3f) - balance >= 3) && (rmse[i] <= 0.7f))
            {
                actualLength++;
                balance += rmse[i];
                //Debug.Log(" B: " + balance + " BU: " + ((i - contIni) * 0.3f));

                //if (((i - contIni) * 0.3f) - balance >= 3)
                //    balance += 2.5f;

                // Si es la mayor longitud hasta ahora guardar la zona
                if (actualLength > bestLength)
                {
                    bestLength = actualLength;
                    timeIniZoneLow = iniZone;
                    timeEndZoneLow = beats[i];
                    beatEndLow = i;
                    beatIniLow = iniCont;
                }
            }
            else
            {
                actualLength = 0;
                balance = 0;
                find = true;
            }
        }
        beatsZones.Add(beatIniLow);
        beatsZones.Add(beatEndLow);
        //Invoke("iniZoneLowMethod", timeIniZoneLow);
        //Invoke("endZoneMethod", timeEndZoneLow);
        //Debug.Log("Ini zona low: " + timeIniZoneLow);
        //Debug.Log("Fin zona low: " + timeEndZoneLow);
    }

    private void Update()
    {
        if (GameManager.instance.getDeath())
        {
            endZoneMethod();
            timeCount = 0;
            for (int i = 0; i < once.Length; i++)
                once[i] = true;
        }

        timeCount += Time.deltaTime;


        if (timeCount >= timeIniZoneLow && once[0])
        {
            iniZoneLowMethod();
            once[0] = false;
        }
        else if (timeCount >= timeEndZoneLow && once[1])
        {
            endZoneMethod();
            once[1] = false;
        }
        if (timeCount >= timeIniZoneHigh && once[2])
        {
            iniZoneHighMethod();
            once[2] = false;
        }
        else if (timeCount >= timeEndZoneLow && once[3])
        {
            endZoneMethod();
            once[3] = false;
        }
    }

    void iniZoneHighMethod()
    {
        RenderSettings.skybox = highSky;
        playerSprite.color = highColor;
    }

    void iniZoneLowMethod()
    {
        RenderSettings.skybox = lowSky;
        playerSprite.color = lowColor;
    }
    void endZoneMethod()
    {
        RenderSettings.skybox = originalSkyBox;
        playerSprite.color = originalColor;
    }

    // GETTERS
    public int getBeatIniHigh() { return beatIniHigh; }
    public int getBeatIniLow() { return beatIniLow; }
    public int getBeatEndHigh() { return beatEndHigh; }
    public int getBeatEndLow() { return beatEndLow; }
    public List<int> getBeatZonesIndexes() { return beatsZones; }
}