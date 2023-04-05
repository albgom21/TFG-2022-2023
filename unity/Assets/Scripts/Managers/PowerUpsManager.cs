using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsManager : MonoBehaviour
{

    public struct PowerUpsData //Información sobre todos los powerUps que se guardarán y cargarán en los checkPoints
    {
        //Los timeLeft indican si el power up está activo o no (si son 0.0f, está desactivado)
        public bool gravity;
        public float slowMotionTimeLeft;
        public float lowResTimeLeft;

        public PowerUpsData(bool g = false, float smtl = 0.0f, float qtl = 0.0f)
        {
            gravity = g;
            slowMotionTimeLeft = smtl;
            lowResTimeLeft = qtl;
        }
    }

    private bool gravityPowerUp, slowMotionPowerUp, lowResPowerUp;
    private float slowMotionTimer, lowResTimer; //Timers que marcan cuanto tiempo queda del powerUp
    private List<GameObject> powerUpsInstances; //Instancias de los power ups para que reaparezcan al morir
    private float smTimeScale; //timeScale de SlowMotion para transicionar lentamente

    [SerializeField]
    private RenderTexture lowRes;       //Textura de renderizado donde se muestra lo que ve la camara con baja calidad
    [SerializeField]
    private RenderTexture highRes;      //Textura de renderizado donde se muestra lo que ve la camara con alta calidad
    [SerializeField]
    private GameObject rawImageLow;     //Imagen donde se muestra la textura de renderizado con baja calidad
    [SerializeField]
    private GameObject rawImageNormal;  //Imagen donde se muestra la textura de renderizado con alta calidad
    [SerializeField]
    private Camera cam;                 //Referencia a la cámara
    [SerializeField]
    private Image powerUpLowImg;
    [SerializeField]
    private Image powerUpSlowImg;

    private bool changedPosX = false;
    private void Awake()
    {
        gravityPowerUp = slowMotionPowerUp = lowResPowerUp = false;
        slowMotionTimer = lowResTimer = 0.0f;
        smTimeScale = 1.0f;

        powerUpsInstances = new List<GameObject>();

        powerUpLowImg.fillAmount = 0;
        powerUpSlowImg.fillAmount = 0;

        if (GameManager.instance != null)
            GameManager.instance.setPowerUpsManager(this);
    }

    // Update is called once per frame
    void Update()
    {
        updateSlowMotion();
        updateLowRes();
        updateHUD();
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
            rawImageNormal.SetActive(true);
            rawImageLow.SetActive(false);
            cam.targetTexture = highRes;
            rawImageNormal.transform.localRotation = new Quaternion(-180, 0, 0, 0);
        }
        else
        {
            //Código de cuando se ha desactivado la gravedad
            rawImageNormal.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    public bool getGravityChanged() { return gravityPowerUp; }

    // ----------------------------------- SLOW MOTION POWER UP --------------------------------

    private void updateSlowMotion()
    {
        if (slowMotionPowerUp)
        {
            //Transición lenta de On
            if (smTimeScale > 0.6f) //Si aún no ha llegado al límite del efecto (x0.6 de velocidad)
            {
                smTimeScale -= 0.3f * Time.deltaTime; //Va bajando la velocidad a ritmo de 0.3 por segundo

                if (smTimeScale < 0.6f) smTimeScale = 0.6f; //Si me paso, recoloco en el límite

                changeTimeScale(smTimeScale);
            }

            //Timer
            slowMotionTimer -= Time.deltaTime;

            if (slowMotionTimer <= 0.0f) slowMotionOff();
        }
        else
        {
            //Transición lenta de Off
            if (smTimeScale < 1.0f) //Si aún no ha llegado a la velocidad normal
            {
                smTimeScale += 0.3f * Time.deltaTime; //Va subiendo la velocidad a ritmo de 0.3 por segundo

                if (smTimeScale > 1.0f) smTimeScale = 1.0f; //Si me paso, recoloco en el límite

                changeTimeScale(smTimeScale);
            }
        }
    }
    public void slowMotionOn(float time)
    {
        slowMotionPowerUp = true;
        slowMotionTimer += time;

        if (lowResPowerUp)
        {
            powerUpLowImg.transform.position -= new Vector3(125, 0, 0);
            changedPosX = true;
        }
    }

    private void slowMotionOff()
    {
        slowMotionPowerUp = false;
        slowMotionTimer = 0.0f;

        powerUpLowImg.fillAmount = 0;

        if (changedPosX)
        {
            powerUpLowImg.transform.position += new Vector3(125, 0, 0);
            changedPosX = false;
        }
    }

    private void changeTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        GameManager.instance.getMusicInstance().setPitch(newTimeScale);
    }

    //Devuelve cuanto tiempo queda de power Up (0 si no está activado), para guardarlo en el checkpoint
    public float getSlowMotionTimer() { return slowMotionTimer; }

    // ----------------------------------- LOW RES POWER UP --------------------------------

    private void updateLowRes()
    {
        if (lowResPowerUp)
        {
            lowResTimer -= Time.deltaTime;

            if (lowResTimer <= 0.0f) lowResOff();
        }
    }

    public void lowResOn(float time)
    {
        lowResPowerUp = true;
        lowResTimer += time;

        cam.targetTexture = lowRes;
        rawImageNormal.SetActive(false);
        rawImageLow.SetActive(true);
        if (gravityPowerUp)
            rawImageLow.transform.localRotation = new Quaternion(-180, 0, 0, 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Quality", 0.0f);
    }

    private void lowResOff()
    {
        lowResPowerUp = false;
        lowResTimer = 0.0f;

        powerUpLowImg.fillAmount = 0;
        cam.targetTexture = highRes;
        rawImageNormal.SetActive(true);
        rawImageLow.SetActive(false);
        rawImageLow.transform.localRotation = new Quaternion(0, 0, 0, 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Quality", 1.0f);

    }

    //Devuelve cuanto tiempo queda de power Up (0 si no está activado), para guardarlo en el checkpoint
    public float getLowResTimer() { return lowResTimer; }

    //------------------------ GUARDADO Y CARGA DE DATOS DE POWER UPS -------------------------

    //Guardado de datos para crear un checkPoint
    public PowerUpsData getData()
    {
        return new PowerUpsData(gravityPowerUp, slowMotionTimer, lowResTimer);
    }

    //Carga de datos del checkPoint al respawnear
    public void resetData(PowerUpsData newData)
    {
        resetInstances();

        //---POWER UP GRAVITY
        if (newData.gravity != gravityPowerUp) changeGravity();

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

        //---POWER UP LOW RES
        if (newData.lowResTimeLeft > 0.0f) //Si en el checkpoint si había lowRes
        {
            if (!lowResPowerUp) lowResOn(newData.lowResTimeLeft); //Y antes de morir no, lo activas
            else lowResTimer = newData.lowResTimeLeft;    //Si antes de morir también lo había, solo hay que cambiar el timer
        }
        else //Si en el checkPoint no había lowRes
        {
            if (lowResPowerUp) lowResOff(); //Y antes de morir sí, lo desactivas
            //else si tampoco lo había antes de morir no haces nada
        }
    }

    private void updateHUD()
    {
        if (lowResPowerUp && lowResTimer > 0)
        {
            powerUpLowImg.fillAmount = lowResTimer / 5.0f;
        }
        if (slowMotionPowerUp && slowMotionTimer > 0)
        {
            powerUpSlowImg.fillAmount = slowMotionTimer / 5.0f;
        }
    }
}
