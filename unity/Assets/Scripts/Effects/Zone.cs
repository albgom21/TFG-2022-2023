using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] private Material high;
    [SerializeField] private Material low;
    [SerializeField] private GameObject features;

    private Material originalSkyBox;

    void Start()
    {
        originalSkyBox = RenderSettings.skybox;

        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        List<float> rmse = features.GetComponent<ReadTxt>().getRMSE();
      
        HighZone(beats, rmse);
        LowZone(beats, rmse);
    }

    private void HighZone(List<float> beats, List<float> rmse)
    {
        float bestIniZone = -1,
             iniZone = -1,
             contIni = -1,
             finZone = -1,
             balance = 0;

        int bestLength = -1;
        int actualLength = 0;

        bool find = true;
        for (int i = 0; i < rmse.Count(); i++)
        {
            // Si está dentro del umbral y la zona no se ha iniciado guardar comienzo
            if (find && rmse[i] >= 0.925f)
            {
                contIni = i;
                iniZone = beats[i];
                find = false;
                balance += rmse[i];
            }
            // Si ya se está en la zona y el balance cumple seguir agrandando la zona
            else if (iniZone >= 0 && balance >= ((i - contIni) * 0.85f) && rmse[i] >= 0.7f)
            {
                //Debug.Log("Rmse: " + rmse[i] + " B: " + balance + " BU: " + ((i - contIni) * 0.85f));
                if ((balance - ((i - contIni) * 0.85f)) >= 2)
                    balance -= 1.75f;

                balance += rmse[i];
                finZone = beats[i];

                actualLength++;
                balance += rmse[i];

                if (actualLength > bestLength)
                {
                    bestLength = actualLength;
                    bestIniZone = iniZone;
                    finZone = beats[i];
                }
            }
            else
            {
                actualLength = 0;
                balance = 0;
                find = true;
            }
        }

        Debug.Log("Ini zona high: " + bestIniZone);
        Debug.Log("Fin zona high: " + finZone);

        Invoke("iniZoneMethod", bestIniZone);
        Invoke("endZoneMethod", finZone);
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

        bool find = true;
        for (int i = 0; i < rmse.Count(); i++)
        {
            // Si está dentro del umbral y la zona no se ha iniciado guardar comienzo
            if (find && (rmse[i] >= 0.4f && rmse[i] < 0.5f))
            {
                //Debug.Log("NUEVA BUSQUEDA en: " + i +" de " + rmse.Count());
                contIni = i;
                iniZone = beats[i];
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
                    bestIniZone = iniZone;
                    finZone = beats[i];
                }
            }
            else
            {
                //Debug.Log("MAX LENGTH: " + bestLength);
                actualLength = 0;
                //Debug.Log("-------FIN------");
                balance = 0;
                find = true;
            }
        }

        Invoke("iniZoneMethod2", bestIniZone);
        Invoke("endZoneMethod", finZone);
        Debug.Log("Ini zona low: " + bestIniZone);
        Debug.Log("Fin zona low: " + finZone);
    }

    void iniZoneMethod()
    {
        RenderSettings.skybox = high;
    }
    void iniZoneMethod2()
    {
        RenderSettings.skybox = low;
    }
    void endZoneMethod()
    {
        RenderSettings.skybox = originalSkyBox;
    }
}