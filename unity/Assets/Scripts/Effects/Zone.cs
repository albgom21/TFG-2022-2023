using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZoneCode;

public class Zone : MonoBehaviour
{
    [SerializeField] private GameObject features;           // Caracteristicas extraidas
    [SerializeField] private SpriteRenderer playerSprite;   // Sprite del jugador
    [SerializeField] Image water;                           // Imagen para efecto agua en zona low
    [SerializeField] private Color highColorPlayer;         // Color del sprite del jugador en zona high
    [SerializeField] private Color lowColorPlayer;          // Color del sprite del jugador en zona low

    private Color originalColorPlayer;                      // Color del sprite original del jugador

    private double timeCount = 0;                           // Contador de tiempo

    public bool waterEnabled = false;                       // Estado del efecto agua

    private List<ZoneData> zData = new List<ZoneData>();    // Lista con los datos de las zonas


    private void Awake()
    {
        GameManager.instance.SetZoneManager(this);
    }

    void Start()
    {
        // Obtener los datos de BPM y RMSE
        List<float> beats = features.GetComponent<ReadTxt>().GetBeatsInTime();
        List<float> rmse = features.GetComponent<ReadTxt>().GetRMSE();

        // Busqueda de zonas con alto y bajo RMSE
        HighZone(beats, rmse);
        LowZone(beats, rmse);

        originalColorPlayer = playerSprite.color;
    }

    private void Update()
    {
        if (GameManager.instance.GetEnd()) return;   // Si el nivel ha terminado

        timeCount += Time.deltaTime;

        // Si el jugador entra en las zonas activarlas o desactivarlas
        for (int i = 0; i < zData.Count; i++)
        {
            // Si no se ha pasado el portal de inicio y ha pasado el tiempo de activación
            if (!zData[i].getActivatedIni() && timeCount >= zData[i].getTimeIniZone() + Constants.DELAY_TIME)
            {
                // Iniciar la zona según su tipo                                    
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

    // Activar la zona segun el tipo
    void IniZone(ZoneType type)
    {
        if (type == ZoneType.HIGH)
            playerSprite.color = highColorPlayer;

        else if (type == ZoneType.LOW)
        {
            waterEnabled = true;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Underwater", 1.0f);
            playerSprite.color = lowColorPlayer;
        }

        GameManager.instance.ChangeZone(type);
    }

    // Desactivar la zona
    void EndZone(bool death = false)
    {
        waterEnabled = false;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Underwater", 0.0f);
        playerSprite.color = originalColorPlayer;
        if (death) water.fillAmount = 0;
        GameManager.instance.ChangeZone(ZoneType.STANDARD);
    }

    private void HighZone(List<float> beats, List<float> rmse)
    {
        ZoneData aux = new ZoneData();

        float iniZone = -1,
              balance = 0;

        int bestLength = -1,
            actualLength = 0,
            iniBeat = -1;

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
                actualLength = 0;
                balance = 0;
                find = true;
            }
        }

        // Añadir si se ha encontrado una zona
        if (bestLength > 0)
        {
            aux.setBeatLength(bestLength);
            aux.setActivatedIni(false);
            aux.setActivatedEnd(false);
            zData.Add(aux);

            // DEBUG
            /* 
            int cont = 0;
            foreach (ZoneData z in zData)
            {
                if (z.getType() == ZoneType.HIGH)
                {
                    cont++;
                    Debug.Log("Z high:" + cont + " ini: " + z.getTimeIniZone() + " end: " + z.getTimeEndZone());
                }
            }
            Debug.Log("BEST High: " + bestLength);
            Debug.Log("Ini zona high: " + zData[zData.Count - 1].getTimeIniZone());
            Debug.Log("Fin zona high: " + zData[zData.Count - 1].getTimeEndZone());
            */
        }
    }

    private void LowZone(List<float> beats, List<float> rmse)
    {
        ZoneData aux = new ZoneData();

        float iniZone = -1,
              balance = 0;

        int bestLength = -1,
            actualLength = 0,
            iniBeat = -1;

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
            }
            // Si ya se está en la zona y el balance cumple seguir agrandando la zona
            else if (iniZone >= 0 && balance <= ((i - iniBeat) * 0.3f) && (((i - iniBeat) * 0.5f) >= balance) && ((((i - iniBeat) * 0.3f) - balance) < 5) && (rmse[i] <= 0.7f))
            {
                actualLength++;
                balance += rmse[i];

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
            aux.setBeatLength(bestLength);
            aux.setActivatedIni(false);
            aux.setActivatedEnd(false);
            zData.Add(aux);

            // DEBUG
            /*
            int cont = 0;
            foreach (ZoneData z in zData)
            {
                if (z.getType() == ZoneType.LOW)
                {
                    cont++;
                    Debug.Log("Z low:" + cont + " ini: " + z.getTimeIniZone() + " end: " + z.getTimeEndZone());
                }
            }
            Debug.Log("BEST low: " + bestLength);
            Debug.Log("Ini zona low: " + zData[zData.Count - 1].getTimeIniZone());
            Debug.Log("Fin zona low: " + zData[zData.Count - 1].getTimeEndZone());
            */
        }
    }

    // GETTERS
    public List<ZoneData> getZonesData() { return zData; }

    public void SyncroAfterPlayerDeath()
    {
        EndZone(true);                                   // Finalizar la zona
        timeCount = GameManager.instance.GetDeathTime(); // Obtener el tiempo de la muerte

        // Desactivar el estado de los inicios y finales de las zonas segun donde se encuentre el jugador
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
}