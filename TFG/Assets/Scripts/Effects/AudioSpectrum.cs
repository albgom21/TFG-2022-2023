using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AudioSpectrum : MonoBehaviour
{
    /*
    
     Clase para recrear el espectro de audio de forma visual

    */

    public float posX = 0;            // Posición de la primera barra
    public float posY = 0;            
    public float posZ = 0;            
                                      
    public int nBars;           // Número de barras
    public GameObject barPrefab;      // Prefab de barra de audio
    public GameObject contenedor;     // Contenedor de las barras
    public ReadTxt samples;          
    private GameObject[] bars;        // Barras totales para el espectro de audio
                                      
    public Color color1;              // Colores por los que pasan las barras   
    public Color color2;              

    public int updatesPerSec;       // Actualizaciones por segundo del tam de las barras
    public float offsetBarsX; // Separación entre barras

    float min = -0.1f;               // Valores min y max de los samples
    float max = 1.2f;

    int sr = 22050;

    List<float> spectrum;
    int cont = 0;
    int aumento;

    void Start()
    {
        sr = samples.getSr();
        aumento = sr / updatesPerSec; // Cantidad de samples que deben salir cada vez que se llama a UpdateSpectrum
        spectrum = samples.getSamples();

        // Crear las barras con un offset en X
        for (int i = 0; i < nBars; i++)
        {
            Instantiate(barPrefab, new Vector3(posX, posY, posZ), barPrefab.transform.rotation, contenedor.transform);
            posX += offsetBarsX;
        }

        // Guardar todas las barras
        bars = GameObject.FindGameObjectsWithTag("Bar");
        InvokeRepeating("UpdateSpectrum", 0.0f, 1.0f / updatesPerSec);
    }

    private void Update()
    {
        // Mover el contenerdor para que siga a la cámara
        contenedor.transform.position = new Vector3(GetComponentInParent<Transform>().position.x, contenedor.transform.position.y, contenedor.transform.position.z);
        
        // SI se ha muerto poner cont = 0
    }

    void UpdateSpectrum()
    {
        //float[] spectrum = new float[1024];          // 64-1024 n muestras
        //AudioListener.GetOutputData(spectrum, 0);    // 0 = Mono

        for (int i = 0; i < nBars; i++)
        {
            // COMPROBAR QUE NO SE SALGA (spectrum[cont + i] > spectrum.Count())
            // Cambio de la escala anterior en función del nuevo valor
            Vector3 prevScale = bars[i].transform.localScale;
            prevScale.y = spectrum[cont+i] * 10;
            bars[i].transform.localScale = prevScale;

            // Cambio de color
            float valueNormalized = (spectrum[cont+i] - min) / (max - min);
            bars[i].GetComponent<Renderer>().material.color = Color.Lerp(color1, color2, valueNormalized);
        }
        cont += aumento; 
    }
}
