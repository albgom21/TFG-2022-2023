using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsManager : MonoBehaviour
{

    public struct PowerUpsData //Informaci�n sobre todos los powerUps que se guardar�n y cargar�n en los checkPoints
    {
        //Los timeLeft indican si el power up est� activo o no (si son 0.0f, est� desactivado)
        public bool gravity;
        public float slowMotionTimeLeft;
        public float lowResTimeLeft;

        public PowerUpsData(bool g = false, float smtl = 0.0f, float qtl = 0.0f) //Por defecto, todos los powerUps desactivados
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
    private Camera cam;                 //Referencia a la c�mara
    [SerializeField]
    private Image powerUpLowImg;
    [SerializeField]
    private Image powerUpSlowImg;

    private Animator cameraAnimator;
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
            GameManager.instance.SetPowerUpsManager(this);
    }

    private void Start()
    {
        cameraAnimator = cam.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSlowMotion();
        UpdateLowRes();

        //if (cam.transform.rotation.eulerAngles != newCameraRotation.eulerAngles)
        //{
        //    Debug.Log(cam.transform.rotation.eulerAngles + "    " + newCameraRotation.eulerAngles);
        //    cam.transform.transform.rotation = Quaternion.Euler(0, 0,
        //            Mathf.MoveTowards(cam.transform.rotation.eulerAngles.z, newCameraRotation.eulerAngles.z, Time.deltaTime * rotationSpeed));
        //    Debug.Log("PRIMERA ROTACI�N");
        //}
    }

    //A�ade una instancia a la lista de powerUps
    public void AddPowerUpInstance(GameObject newPowerUp)
    {
        powerUpsInstances.Add(newPowerUp);
    }

    //Resetea todos los power ups (vuelven a aparecer cuando mueres)
    public void ResetInstances()
    {
        for (int i = 0; i < powerUpsInstances.Count; ++i)
        {
            powerUpsInstances[i].GetComponent<PowerUp>().ResetPowerUp();
        }
    }

    // ----------------------------------- GRAVITY POWER UP --------------------------------

    public void ChangeGravity()
    {
        gravityPowerUp = !gravityPowerUp;

        if (gravityPowerUp)
        {
            //C�digo de cuando se ha activado la gravedad
            rawImageNormal.SetActive(true);
            rawImageLow.SetActive(false);
            cam.targetTexture = highRes;
            cameraAnimator.SetBool("GravityOn", true);

        }
        else //C�digo de cuando se ha desactivado la gravedad
            cameraAnimator.SetBool("GravityOn", false);

        
    }

    public bool GetGravityChanged() { return gravityPowerUp; }

    //Reseteo de gravity al Respawnear
    private void ResetGravity(bool newGravity)
    {
        if (newGravity != gravityPowerUp) ChangeGravity();
    }

    // ----------------------------------- SLOW MOTION POWER UP --------------------------------

    private void UpdateSlowMotion()
    {
        if (slowMotionPowerUp)
        {
            //Transición lenta de On
            if (smTimeScale > 0.6f) //Si a�n no ha llegado al l�mite del efecto (x0.6 de velocidad)
            {
                smTimeScale -= 0.3f * Time.deltaTime; //Va bajando la velocidad a ritmo de 0.3 por segundo

                if (smTimeScale < 0.6f) smTimeScale = 0.6f; //Si me paso, recoloco en el l�mite

                ChangeTimeScale(smTimeScale);
            }

            //Timer
            slowMotionTimer -= Time.deltaTime;

            if (slowMotionTimer <= 0.0f) SlowMotionOff();
        }
        else
        {
            //Transici�n lenta de Off
            if (smTimeScale < 1.0f) //Si a�n no ha llegado a la velocidad normal
            {
                smTimeScale += 0.3f * Time.deltaTime; //Va subiendo la velocidad a ritmo de 0.3 por segundo

                if (smTimeScale > 1.0f) smTimeScale = 1.0f; //Si me paso, recoloco en el l�mite

                ChangeTimeScale(smTimeScale);
            }
        }

        //HUD
        powerUpLowImg.fillAmount = lowResTimer / 5.0f;
    }
    public void SlowMotionOn(float time)
    {
        slowMotionPowerUp = true;
        slowMotionTimer = time;

        if (lowResPowerUp)
        {
            powerUpLowImg.transform.position -= new Vector3(125, 0, 0);
            changedPosX = true;
        }
    }

    private void SlowMotionOff()
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

    private void ChangeTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        GameManager.instance.GetMusicInstance().setPitch(newTimeScale);
    }


    //Devuelve cuanto tiempo queda de power Up (0 si no est� activado), para guardarlo en el checkpoint
    public float GetSlowMotionTimer() { return slowMotionTimer; }

    //Resetea el efecto SlowMotion al respawnear
    private void ResetSlowMotion(float newTime)
    {
        if (newTime > 0.0f) //Si en el checkpoint si hab�a SlowMotion
        {
            slowMotionTimer = newTime;
            if (!slowMotionPowerUp) //Si al morir, no ten�as el powerUp activado
            {
                //Cambia a activado SIN transici�n
                slowMotionPowerUp = true;
                smTimeScale = 0.6f;
                ChangeTimeScale(smTimeScale);
            }

            //CAMBIO DE HUD, A�ADIENDO EL POWER UP SI NO LO ESTABA O CAMBIANDOLO A newTime SI LO HAB�A ANTES TAMBI�N

        }
        else if (slowMotionPowerUp)//Si en el checkPoint no hab�a SlowMotion y al morir si lo ten�as
        {
            slowMotionTimer = 0.0f;
            slowMotionPowerUp = false;
            smTimeScale = 1.0f;
            ChangeTimeScale(smTimeScale);

            //CAMBIO HUD, QUITANDO EL POWER UP


        }
        powerUpSlowImg.fillAmount = slowMotionTimer / 5.0f;

        //En el caso de no tenerlo ni en el checkpoint ni al morir, no hay que hacer nada.
    }

    // ----------------------------------- LOW RES POWER UP --------------------------------

    private void UpdateLowRes()
    {
        if (lowResPowerUp)
        {
            lowResTimer -= Time.deltaTime;

            if (lowResTimer <= 0.0f) LowResOff();
        }

        //HUD
        powerUpSlowImg.fillAmount = slowMotionTimer / 5.0f;
    }

    public void LowResOn(float time)
    {
        lowResPowerUp = true;
        lowResTimer = time;

        cam.targetTexture = lowRes;
        rawImageNormal.SetActive(false);
        rawImageLow.SetActive(true);
        if (gravityPowerUp)
            rawImageLow.transform.localRotation = new Quaternion(-180, 0, 0, 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Quality", 0.0f);

        if (slowMotionPowerUp)
        {
            powerUpLowImg.transform.position -= new Vector3(125, 0, 0);
            changedPosX = true;
        }
    }

    private void LowResOff()
    {
        lowResPowerUp = false;
        lowResTimer = 0.0f;

        powerUpLowImg.fillAmount = 0;
        cam.targetTexture = highRes;
        rawImageNormal.SetActive(true);
        rawImageLow.SetActive(false);
        rawImageLow.transform.localRotation = new Quaternion(0, 0, 0, 0);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Quality", 1.0f);

        if (changedPosX)
        {
            powerUpLowImg.transform.position += new Vector3(125, 0, 0);
            changedPosX = false;
        }
    }

    //Devuelve cuanto tiempo queda de power Up (0 si no est� activado), para guardarlo en el checkpoint
    public float GetLowResTimer() { return lowResTimer; }

    //Resetea el efecto LowRes al respawnear
    private void ResetLowRes(float newTime)
    {
        if (newTime > 0.0f) //Si en el checkpoint si hab�a LowRes
        {
            lowResTimer = newTime; //Cambio el tiempo que queda (independientemente de si antes estaba activado o no)

            if (!lowResPowerUp) //Si al morir, no ten�as el powerUp activado, lo activas
            {
                LowResOn(newTime);
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Quality", 0.0f, true); //Cambio SIN TRANSICI�N
            }

        }
        else if (lowResPowerUp)//Si en el checkPoint no hab�a losRes y al morir si lo ten�as
        {
            LowResOff();
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Quality", 1.0f, true); //Cambio SIN TRANSICI�N

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
    public void ResetData(PowerUpsData newData)
    {
        ResetInstances(); //Hacer reaparecer las instancias de los powerups

        //Resetear el efecto de Gravity
        ResetGravity(newData.gravity);

        //Resetear el efecto de SlowMotion
        ResetSlowMotion(newData.slowMotionTimeLeft);

        //Resetear el efecto de LowRes
        ResetLowRes(newData.lowResTimeLeft);
    }
}