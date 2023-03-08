using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] private Material skyboxNew;
    [SerializeField] private GameObject features;

    private Material skyboxOld;

    void Start()
    {
        skyboxOld = RenderSettings.skybox;

        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        List<float> rmse = features.GetComponent<ReadTxt>().getRMSE();

        float iniZone = -1;
        float contIni = -1;
        float finZone = -1;
        float balance = 0;

        bool find = true;
        for (int i = 0; i < rmse.Count(); i++)
        {
            // Si está dentro del umbral y la zona no se ha iniciado guardar comienzo
            if (find && rmse[i] >= 0.925f && iniZone < 0)
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
            }
            else
            {
                balance = 0;
                find = true;
            }
        }

        //Debug.Log("Ini zona: " + iniZone);
        //Debug.Log("Fin zona: " + finZone);

        Invoke("iniZoneMethod", iniZone);
        Invoke("endZoneMethod", finZone);
    }
    void iniZoneMethod()
    {
        RenderSettings.skybox = skyboxNew;
    }
    void endZoneMethod()
    {
        RenderSettings.skybox = skyboxOld;
    }
}