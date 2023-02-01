using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrum : MonoBehaviour
{
    /*
     Clase para recrear el espectro de audio de forma visual
    */

    public float posX = 0;
    public float posY = 0;
    public float posZ = 0;

    public int nBars;             // Número de barras
    public GameObject barPrefab;  // Prefab de barra de audio
    public GameObject contenedor; // Contenedor de las barras
    private GameObject[] bars;    // Barras totales para el espectro de audio

    public Color color1;
    public Color color2;

    float min = -0.01f;
    float max = 0.1f;
    public int updatesPerSec = 60;

    void Start()
    {
        for (int i = 0; i < nBars; i++)
        {
            Instantiate(barPrefab, new Vector3(posX, posY, posZ), Quaternion.identity, contenedor.transform);
            posX += 0.1f;
        }
        // Guardar todas las barras
        bars = GameObject.FindGameObjectsWithTag("Bar");
        InvokeRepeating("UpdateSpectrum", 1.0f / updatesPerSec, 1.0f / updatesPerSec);
    }

    private void Update()
    {
        contenedor.transform.position = new Vector3(GetComponentInParent<Transform>().position.x, contenedor.transform.position.y, contenedor.transform.position.z);
    }

    void UpdateSpectrum()
    {
        float[] spectrum = new float[1024];          // 64-1024 n muestras
        AudioListener.GetOutputData(spectrum, 0);    // 0 = Mono

        for (int i = 0; i < nBars; i++)
        {
            //Cambio de la escala anterior en función del nuevo valor
            Vector3 prevScale = bars[i].transform.localScale;
            prevScale.y = spectrum[i] * 50;
            bars[i].transform.localScale = prevScale;
            float valueNormalized = (spectrum[i] - min) / (max - min);
            bars[i].GetComponent<Renderer>().material.color = Color.Lerp(color1, color2, valueNormalized);
        }
    }
}
