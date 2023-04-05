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

    //Reseteo de gravity al Respawnear
    private void resetGravity(bool newGravity)
    {
        if (newGravity != gravityPowerUp) changeGravity();
    }

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

        //HUD
        if (lowResPowerUp && lowResTimer > 0)
        {
            powerUpLowImg.fillAmount = lowResTimer / 5.0f;
        }
    }
    public void slowMotionOn(float time)
    {
        slowMotionPowerUp = true;
        slowMotionTimer = time;

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

    //Resetea el efecto SlowMotion al respawnear
    private void resetSlowMotion(float newTime)
    {
        if (newTime > 0.0f) //Si en el checkpoint si había SlowMotion
        {
            slowMotionTimer = newTime;
            if (!slowMotionPowerUp) //Si al morir, no tenías el powerUp activado
            {
                //Cambia a activado SIN transición
                slowMotionPowerUp = true;
                smTimeScale = 0.6f;
                changeTimeScale(smTimeScale);
            }

            //CAMBIO DE HUD, AÑADIENDO EL POWER UP SI NO LO ESTABA O CAMBIANDOLO A newTime SI LO HABÍA ANTES TAMBIÉN

        }
        else if (slowMotionPowerUp)//Si en el checkPoint no había SlowMotion y al morir si lo tenías
        {
            slowMotionTimer = 0.0f;
            slowMotionPowerUp = false;
            smTimeScale = 1.0f;
            changeTimeScale(smTimeScale);

            //CAMBIO HUD, QUITANDO EL POWER UP

            
        }

        //En el caso de no tenerlo ni en el checkpoint ni al morir, no hay que hacer nada.
    }

    // ----------------------------------- LOW RES POWER UP --------------------------------

    private void updateLowRes()
    {
        if (lowResPowerUp)
        {
            lowResTimer -= Time.deltaTime;

            if (lowResTimer <= 0.0f) lowResOff();
        }

        //HUD
        if (slowMotionPowerUp && slowMotionTimer > 0.0f)
        {
            powerUpSlowImg.fillAmount = slowMotionTimer / 5.0f;
        }
    }

    public void lowResOn(float time)
    {
        lowResPowerUp = true;
        lowResTimer = time;

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

    //Resetea el efecto LowRes al respawnear
    private void resetLowRes(float newTime)
    {
        if (newTime > 0.0f) //Si en el checkpoint si había LowRes
        {
            lowResTimer = newTime; //Cambio el tiempo que queda (independientemente de si antes estaba activado o no)

            if (!lowResPowerUp) //Si al morir, no tenías el powerUp activado, lo activas
            {
                lowResOn(newTime);
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Quality", 0.0f, true); //Cambio SIN TRANSICIÓN
            } 

        }
        else if (lowResPowerUp)//Si en el checkPoint no había losRes y al morir si lo tenías
        {
            lowResOff();
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Quality", 1.0f, true); //Cambio SIN TRANSICIÓN

        }

        //En el caso de no tenerlo ni en el checkpoint ni al morir, no hay que hacer nada.
    }

    //------------------------ GUARDADO Y CARGA DE DATOS DE POWER UPS -------------------------

    //Guardado de datos para crear un checkPoint
    public PowerUpsData getData()
    {
        return new PowerUpsData(gravityPowerUp, slowMotionTimer, lowResTimer);
    }

    //Carga de datos del checkPoint al respawnear
    public void resetData(PowerUpsData newData)
    {
        resetInstances(); //Hacer reaparecer las instancias de los powerups

        //Resetear el efecto de Gravity
        resetGravity(newData.gravity);

        //Resetear el efecto de SlowMotion
        resetSlowMotion(newData.slowMotionTimeLeft);

        //Resetear el efecto de LowRes
        resetLowRes(newData.lowResTimeLeft);
        
    }
}
