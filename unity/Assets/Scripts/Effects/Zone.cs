using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// TO DO
// CAMBIAR STRUCTS A UN SCRIPT UNICO DE STRUCTS

namespace dataStructs
{
    public class Zone : MonoBehaviour
    {
        [SerializeField] private Material highSky;
        [SerializeField] private Material lowSky;
        [SerializeField] private GameObject features;
        [SerializeField] private SpriteRenderer playerSprite;
        [SerializeField] private Color highColor;
        [SerializeField] private Color lowColor;

        FMOD.Studio.EventInstance eventInstance;

        private Color originalColor;

        private Material originalSkyBox;

        private List<int> beatsZones = new List<int>();

        private double timeCount = 0;

        [SerializeField] Image water;
        public bool waterEnabled = false;


        public enum ZoneType { HIGH, LOW };
        public struct ZoneData
        {
            ZoneType type;

            int beatLength;
            int beatIni;
            int beatEnd;
            float timeIniZone;
            float timeEndZone;
            public bool activatedIni;
            bool activatedEnd;

            public ZoneData(ZoneType t, int blength, int bIni, int bEnd, float tIni, float tEnd, bool actIni, bool actEnd)
            {
                type = t;
                beatLength = blength;
                beatIni = bIni;
                beatEnd = bEnd;
                timeIniZone = tIni;
                timeEndZone = tEnd;
                activatedIni = actIni;
                activatedEnd = actEnd;
            }

            //Getters
            public ZoneType getType() { return type; }
            public int getBeatLength() { return beatLength; }
            public int getBeatIni() { return beatIni; }
            public int getBeatEnd() { return beatEnd; }
            public float getTimeIniZone() { return timeIniZone; }
            public float getTimeEndZone() { return timeEndZone; }
            public bool getActivatedIni() { return activatedIni; }
            public bool getActivatedEnd() { return activatedEnd; }

            //Setters
            public void setType(ZoneType t) { type = t; }
            public void setBeatLength(int i) { beatLength = i; }
            public void setBeatIni(int i) { beatIni = i; }
            public void setBeatEnd(int i) { beatEnd = i; }
            public void setTimeIniZone(float i) { timeIniZone = i; }
            public void setTimeEndZone(float i) { timeEndZone = i; }
            public void setActivatedIni(bool i) { activatedIni = i; }
            public void setActivatedEnd(bool i) { activatedEnd = i; }

        }
        private List<ZoneData> zData = new List<ZoneData>();

        void Start()
        {
            originalSkyBox = RenderSettings.skybox;

            List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
            List<float> rmse = features.GetComponent<ReadTxt>().getRMSE();

            HighZone(beats, rmse);
            LowZone(beats, rmse);

            originalColor = playerSprite.color;
        }

        private void Update()
        {
            if (!GameManager.instance.getEnd())
            {
                if (GameManager.instance.getDeath())
                {
                    endZone(true);
                    timeCount = GameManager.instance.getDeathTime();
                    for (int i = 0; i < zData.Count; i++)
                    {
                        ZoneData aux = zData[i];
                        if (timeCount < zData[i].getTimeIniZone() || (timeCount > zData[i].getTimeIniZone() && timeCount < zData[i].getTimeEndZone()))
                            aux.setActivatedIni(false);
                        if (timeCount < zData[i].getTimeEndZone() || (timeCount > zData[i].getTimeIniZone() && timeCount < zData[i].getTimeEndZone()))
                            aux.setActivatedEnd(false);
                        zData[i] = aux;
                    }
                }

                timeCount += Time.deltaTime;
                for (int i = 0; i < zData.Count; i++)
                {
                    // Si no se ha pasado el portal de inicio y ha pasado el tiempo de activación
                    if (!zData[i].getActivatedIni() && timeCount >= zData[i].getTimeIniZone())
                    {
                        // Poner la zona según su tipo                                    
                        iniZone(zData[i].getType());

                        ZoneData aux = zData[i];
                        aux.setActivatedIni(true);
                        zData[i] = aux;

                        break;
                    }
                    // Si no se había salido de la zona y ha pasado el tiempo de la zona
                    else if (!zData[i].getActivatedEnd() && timeCount >= zData[i].getTimeEndZone())
                    {
                        // Finalizar la zona
                        endZone();

                        ZoneData aux = zData[i];
                        aux.setActivatedEnd(true);
                        zData[i] = aux;

                        break;
                    }
                }
                // Efecto agua en zona LOW
                if (waterEnabled && water.fillAmount < 1)
                    water.fillAmount += Time.deltaTime * 2;
                else if (!waterEnabled && water.fillAmount > 0)
                    water.fillAmount -= Time.deltaTime * 2;
            }
        }
        void iniZone(ZoneType type)
        {
            if (type == ZoneType.HIGH)
            {
                RenderSettings.skybox = highSky;
                playerSprite.color = highColor;
            }
            else if (type == ZoneType.LOW)
            {
                waterEnabled = true;
                eventInstance.setParameterByName("Underwater", 1);
                RenderSettings.skybox = lowSky;
                playerSprite.color = lowColor;
            }
        }

        void endZone(bool death = false)
        {
            waterEnabled = false;
            eventInstance.setParameterByName("Underwater", 0);
            RenderSettings.skybox = originalSkyBox;
            playerSprite.color = originalColor;
            if (death)
                water.fillAmount = 0;
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
                if (find && rmse[i] >= 0.925f)
                {
                    iniBeat = i;
                    iniZone = beats[i];
                    find = false;
                    balance += rmse[i];
                }
                // Si ya se está en la zona y el balance cumple seguir agrandando la zona
                else if (iniZone >= 0 && balance >= ((i - iniBeat) * 0.85f) && rmse[i] >= 0.7f)
                {
                    //Debug.Log("Rmse: " + rmse[i] + " B: " + balance + " BU: " + ((i - contIni) * 0.85f));
                    if ((balance - ((i - iniBeat) * 0.85f)) >= 2)
                        balance -= 1.75f;

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
                else if (iniZone >= 0 && balance <= ((i - iniBeat) * 0.3f) && (((i - iniBeat) * 0.3f) - balance >= 3) && (rmse[i] <= 0.7f))
                {
                    //Debug.Log("ampliando zona: " + iniZone + " i: " + i);
                    actualLength++;
                    balance += rmse[i];
                    //Debug.Log(" B: " + balance + " BU: " + ((i - iniBeat) * 0.3f)+" i: "+ i +" iniB: "+iniBeat);

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
            aux.setType(ZoneType.LOW);
            aux.setBeatLength(actualLength);
            aux.setActivatedIni(false);
            aux.setActivatedEnd(false);
            zData.Add(aux);

            beatsZones.Add(zData[zData.Count - 1].getBeatIni());
            beatsZones.Add(zData[zData.Count - 1].getBeatEnd());

            //int cont = 0;
            //foreach(ZoneData z in zData)
            //{
            //    if(z.getType() == ZoneType.LOW)
            //    {
            //        cont++;
            //        Debug.Log("Z low:" +cont+" ini: "+z.getTimeIniZone()+ " end: " +z.getTimeEndZone());
            //    }
            //}
            //Debug.Log("BEST low: " + bestLength);
            //Debug.Log("Ini zona low: " + zData[zData.Count - 1].getTimeIniZone());
            //Debug.Log("Fin zona low: " + zData[zData.Count - 1].getTimeEndZone());
        }

        public void setEventInstance(FMOD.Studio.EventInstance eI)
        {
            eventInstance = eI;
        }

        // GETTERS
        //public int getBeatIniHigh() { return beatIniHigh; }
        //public int getBeatIniLow() { return beatIniLow; }
        //public int getBeatEndHigh() { return beatEndHigh; }
        //public int getBeatEndLow() { return beatEndLow; }
        public List<ZoneData> getZonesData() { return zData; }
        public List<int> getBeatsZonesIndex() { return beatsZones; }
    }
}