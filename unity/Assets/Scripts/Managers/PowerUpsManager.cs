using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsManager : MonoBehaviour
{
    
    public struct PowerUpsData //Información sobre todos los powerUps que se guardarán y cargarán en los checkPoints
    {
        //Los timeLeft indican si el power up está activo o no (si son 0.0f, está desactivado)
        public bool gravity;                
        public float slowMotionTimeLeft;
        public float qualityTimeLeft;

        public PowerUpsData(bool g = false, float smtl = 0.0f, float qtl = 0.0f)
        {
            gravity = g;
            slowMotionTimeLeft = smtl;
            qualityTimeLeft = qtl;
        }
    }

    private bool gravityPowerUp, slowMotionPowerUp, qualityPowerUp;
    private float slowMotionTimer, qualityTimer; //Timers que marcan cuanto tiempo queda del powerUp
    private List<GameObject> powerUpsInstances; //Instancias de los power ups para que reaparezcan al morir

    private void Awake()
    {
        gravityPowerUp = slowMotionPowerUp = qualityPowerUp = false;
        slowMotionTimer = qualityTimer = 0.0f;

        powerUpsInstances = new List<GameObject>();

        if (GameManager.instance != null)
            GameManager.instance.setPowerUpsManager(this);
    }

    // Update is called once per frame
    void Update()
    {
        updateSlowMotion();
        updateQuality();
    }

    //Añade una instancia a la lista de powerUps
    public void addPowerUpInstance(GameObject newPowerUp)
    {
        powerUpsInstances.Add(newPowerUp);
    }

    //Resetea todos los power ups (vuelven a aparecer cuando mueres)
    public void resetInstances()
    {
        for (int i = 0; i < powerUpsInstances.Count; ++i)
        {
            powerUpsInstances[i].GetComponent<PowerUp>().resetPowerUp();
        }
    }

    // ----------------------------------- GRAVITY POWER UP --------------------------------

    public void changeGravity()
    {
        gravityPowerUp = !gravityPowerUp;

        if (gravityPowerUp)
        {
            //Código de cuando se ha activado la gravedad

        }
        else
        {
            //Código de cuando se ha desactivado la gravedad

        }
    }

    public bool getGravityChanged() { return gravityPowerUp; }

    //Resetea el powerUp al respawnear
    public void restartGravity(bool gravity) { gravityPowerUp = gravity; }

    // ----------------------------------- SLOW MOTION POWER UP --------------------------------

    private void updateSlowMotion()
    {
        if (slowMotionPowerUp)
        {
            slowMotionTimer -= Time.deltaTime;

            if (slowMotionTimer <= 0.0f) slowMotionOff();
        }
    }
    public void slowMotionOn(float time)
    {
        slowMotionPowerUp = true;
        slowMotionTimer += time;

        changeTimeScale(0.7f);
    }

    private void slowMotionOff()
    {
        slowMotionPowerUp = false;
        slowMotionTimer = 0.0f;

        changeTimeScale(1.0f);
    }

    private void changeTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        GameManager.instance.getMusicInstance().setPitch(newTimeScale);
    }

    //Devuelve cuanto tiempo queda de power Up (0 si no está activado), para guardarlo en el checkpoint
    public float getSlowMotionTimer() { return slowMotionTimer; }
        
    // ----------------------------------- BAD QUALITY POWER UP --------------------------------

    private void updateQuality()
    {
        if (qualityPowerUp)
        {
            qualityTimer -= Time.deltaTime;

            if (qualityTimer <= 0.0f) badQualityOff();
        }
    }


    public void badQualityOn(float time)
    {
        qualityPowerUp = false;
        qualityTimer += time;

        Transform tr = GameObject.FindWithTag("RawImage").GetComponent<Transform>();
        tr.localRotation = new Quaternion(-180, 0, 0, 0);
    }

    private void badQualityOff()
    {
        qualityPowerUp = false;
        qualityTimer = 0.0f;

        Transform tr = GameObject.FindWithTag("RawImage").GetComponent<Transform>();
        tr.localRotation = new Quaternion(0, 0, 0, 0);
    }

    //Devuelve cuanto tiempo queda de power Up (0 si no está activado), para guardarlo en el checkpoint
    public float getBadQualityTimer() { return qualityTimer; }

    //------------------------ GUARDADO Y CARGA DE DATOS DE POWER UPS -------------------------

    //Guardado de datos para crear un checkPoint
    public PowerUpsData getData()
    {
        return new PowerUpsData(gravityPowerUp, slowMotionTimer, qualityTimer);
    }

    //Carga de datos del checkPoint al respawnear
    public void resetData(PowerUpsData newData)
    {
        resetInstances();

        //---POWER UP GRAVITY
        gravityPowerUp = newData.gravity;

        //---POWER UP SLOWMOTION
        if (newData.slowMotionTimeLeft > 0.0f) //Si en el checkpoint si había SlowMotion
        {
            if (!slowMotionPowerUp) slowMotionOn(newData.slowMotionTimeLeft); //Y antes de morir no, lo activas
            else slowMotionTimer = newData.slowMotionTimeLeft;    //Si antes de morir también lo había, solo hay que cambiar el timer
        }
        else //Si en el checkPoint no había SlowMotion
        {
            if (slowMotionPowerUp) slowMotionOff(); //Y antes de morir sí, lo desactivas
            //else si tampoco lo había antes de morir no haces nada
        }

        //---POWER UP BAD QUALITY
        if (newData.qualityTimeLeft > 0.0f) //Si en el checkpoint si había badQuality
        {
            if (!qualityPowerUp) badQualityOn(newData.qualityTimeLeft); //Y antes de morir no, lo activas
            else qualityTimer = newData.qualityTimeLeft;    //Si antes de morir también lo había, solo hay que cambiar el timer
        }
        else //Si en el checkPoint no había badQuality
        {
            if (qualityPowerUp) badQualityOff(); //Y antes de morir sí, lo desactivas
            //else si tampoco lo había antes de morir no haces nada
        }
    }

}
