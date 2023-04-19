using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZoneCode;

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
    private double timeCount = 0;
    [SerializeField] Image water;
    public bool waterEnabled = false;
    private List<ZoneData> zData = new List<ZoneData>();


    void Start()
    {
        originalSkyBox = RenderSettings.skybox;

        List<float> beats = features.GetComponent<ReadTxt>().GetBeatsInTime();
        List<float> rmse = features.GetComponent<ReadTxt>().GetRMSE();

        HighZone(beats, rmse);
        LowZone(beats, rmse);

        originalColor = playerSprite.color;
    }

    private void Update()
    {
        if (GameManager.instance.GetEnd()) return;

        if (GameManager.instance.GetDeath())
        {
            EndZone(true);
            timeCount = GameManager.instance.GetDeathTime();
            for (int i = 0; i < zData.Count; i++)
            {
                ZoneData aux = zData[i];
                if (timeCount < zData[i].getTimeIniZone() + Constants.DELAY_TIME || (timeCount > zData[i].getTimeIniZone() + Constants.DELAY_TIME && timeCount < zData[i].getTimeEndZone() + Constants.DELAY_TIME))
                    aux.setActivatedIni(false);
                if (timeCount < zData[i].getTimeEndZone() + Constants.DELAY_TIME || (timeCount > zData[i].getTimeIniZone() + Constants.DELAY_TIME && timeCount < zData[i].getTimeEndZone() + Constants.DELAY_TIME))
                    aux.setActivatedEnd(false);
                zData[i] = aux;
            }
        }

        timeCount += Time.deltaTime;
        for (int i = 0; i < zData.Count; i++)
        {
            // Si no se ha pasado el portal de inicio y ha pasado el tiempo de activación
            if (!zData[i].getActivatedIni() && timeCount >= zData[i].getTimeIniZone() + Constants.DELAY_TIME)
            {
                // Poner la zona según su tipo                                    
                IniZone(zData[i].getType());

                ZoneData aux = zData[i];
                aux.setActivatedIni(true);
                zData[i] = aux;

                break;
            }
            // Si no se había salido de la zona y ha pasado el tiempo de la zona
            else if (!zData[i].getActivatedEnd() && timeCount >= zData[i].getTimeEndZone() + Constants.DELAY_TIME)
            {
                // Finalizar la zona
                EndZone();

                ZoneData aux = zData[i];
                aux.setActivatedEnd(true);
                zData[i] = aux;

                break;
            }
        }
        // Efecto agua en zona LOW
        if (waterEnabled && water.fillAmount < 1) water.fillAmount += Time.deltaTime;
        else if (!waterEnabled && water.fillAmount > 0) water.fillAmount -= Time.deltaTime;

    }

    void IniZone(ZoneType type)
    {
        if (type == ZoneType.HIGH)
        {
            RenderSettings.skybox = highSky;
            playerSprite.color = highColor;
        }
        else if (type == ZoneType.LOW)
        {
            waterEnabled = true;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Underwater", 1.0f);
            RenderSettings.skybox = lowSky;
            playerSprite.color = lowColor;
        }
        GameManager.instance.ChangeZone(type);
    }

    void EndZone(bool death = false)
    {
        waterEnabled = false;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Underwater", 0.0f);
        RenderSettings.skybox = originalSkyBox;
        playerSprite.color = originalColor;
        if (death) water.fillAmount = 0;
        GameManager.instance.ChangeZone(ZoneType.STANDARD);
    }

    private void HighZone(List<float> beats, List<float> rmse)
    {
        ZoneData aux = new ZoneData();

        float iniZone = -1,
              balance = 0;

        int bestLength = -1;
        int actualLength = 0;
        int iniBeat = -1;

        bool find = true;
        for (int i = 0; i < rmse.Count(); i++)
        {
            // Si está dentro del umbral y la zona no se ha iniciado guardar comienzo
            if (find && rmse[i] >= 0.9f)
            {
                iniBeat = i;
                iniZone = beats[i];
                find = false;
                balance += rmse[i];
            }
            // Si ya se está en la zona y el balance cumple seguir agrandando la zona
            else if (iniZone >= 0 && balance >= ((i - iniBeat) * 0.80f) && rmse[i] >= 0.7f)
            {
                //Debug.Log("Rmse: " + rmse[i] + " B: " + balance + " BU: " + ((i - contIni) * 0.80f));
                if ((balance - ((i - iniBeat) * 0.80f)) >= 2)
                    balance -= 1.70f;

                actualLength++;
                balance += rmse[i];

                if (actualLength > bestLength)
                {
                    bestLength = actualLength;
                    aux.setType(ZoneType.HIGH);
                    aux.setTimeIniZone(iniZone);
                    aux.setTimeEndZone(beats[i]);
                    aux.setBeatIni(iniBeat);
                    aux.setBeatEnd(i);
                }
            }
            else
            {
                //if (actualLength != 0)
                //    Debug.Log("Al h: " + actualLength);
                actualLength = 0;
                balance = 0;
                find = true;
            }
        }

        // Añadir si se ha encontrado una zona
        if (bestLength > 0)
        {
            aux.setBeatLength(actualLength);
            aux.setActivatedIni(false);
            aux.setActivatedEnd(false);
            zData.Add(aux);

            beatsZones.Add(zData[zData.Count - 1].getBeatIni());
            beatsZones.Add(zData[zData.Count - 1].getBeatEnd());

            //int cont = 0;
            //foreach (ZoneData z in zData)
            //{
            //    if (z.getType() == ZoneType.HIGH)
            //    {
            //        cont++;
            //        Debug.Log("Z high:" + cont + " ini: " + z.getTimeIniZone() + " end: " + z.getTimeEndZone());
            //    }
            //}
            //Debug.Log("BEST High: " + bestLength);

            //Debug.Log("Ini zona high: " + zData[zData.Count - 1].getTimeIniZone());
            //Debug.Log("Fin zona high: " + zData[zData.Count - 1].getTimeEndZone());
        }
    }

    private void LowZone(List<float> beats, List<float> rmse)
    {
        ZoneData aux = new ZoneData();

        float iniZone = -1,
              balance = 0;

        int bestLength = -1;
        int actualLength = 0;
        int iniBeat = -1;


        bool find = true;
        for (int i = 0; i < rmse.Count(); i++)
        {
            // Si está dentro del umbral y la zona no se ha iniciado guardar comienzo
            if (find && (rmse[i] >= 0.4f && rmse[i] < 0.5f))
            {
                iniBeat = i;
                iniZone = beats[i];
                find = false;
                balance += rmse[i];
                //Debug.Log("iniZone: " + iniZone + " i: "+ i);
            }
            // Si ya se está en la zona y el balance cumple seguir agrandando la zona
            else if (iniZone >= 0 && balance <= ((i - iniBeat) * 0.3f) && (((i - iniBeat) * 0.5f) >= balance) && ((((i - iniBeat) * 0.3f) - balance) < 5) && (rmse[i] <= 0.7f))
            {
                //Debug.Log("ampliando zona: " + iniZone + " i: " + i);
                actualLength++;
                balance += rmse[i];
                //Debug.Log(" B: " + balance + " BU: " + ((i - iniBeat) * 0.3f) + " BU2: " + ((((i - iniBeat) * 0.5f) - balance)) /*+ " i: "+ i +" iniB: "+iniBeat*/);

                //if (((i - contIni) * 0.3f) - balance >= 3)
                //    balance += 2.5f;

                // Si es la mayor longitud hasta ahora guardar la zona
                if (actualLength > bestLength)
                {
                    bestLength = actualLength;
                    aux.setTimeIniZone(iniZone);
                    aux.setTimeEndZone(beats[i]);
                    aux.setBeatIni(iniBeat);
                    aux.setBeatEnd(i);
                }
            }
            else
            {
                //Debug.Log("Nada i: " + i);
                actualLength = 0;
                balance = 0;
                find = true;
            }
        }

        // Añadir si se ha encontrado una zona
        if (bestLength > 0)
        {
            aux.setType(ZoneType.LOW);
            aux.setBeatLength(actualLength);
            aux.setActivatedIni(false);
            aux.setActivatedEnd(false);
            zData.Add(aux);

            beatsZones.Add(zData[zData.Count - 1].getBeatIni());
            beatsZones.Add(zData[zData.Count - 1].getBeatEnd());

            //int cont = 0;
            //foreach (ZoneData z in zData)
            //{
            //    if (z.getType() == ZoneType.LOW)
            //    {
            //        cont++;
            //        Debug.Log("Z low:" + cont + " ini: " + z.getTimeIniZone() + " end: " + z.getTimeEndZone());
            //    }
            //}
            //Debug.Log("BEST low: " + bestLength);
            //Debug.Log("Ini zona low: " + zData[zData.Count - 1].getTimeIniZone());
            //Debug.Log("Fin zona low: " + zData[zData.Count - 1].getTimeEndZone());
        }
    }

    // GETTERS
    public List<ZoneData> getZonesData() { return zData; }
    public List<int> getBeatsZonesIndex() { return beatsZones; }
}