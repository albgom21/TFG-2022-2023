using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadTxt : MonoBehaviour
{
   /*
    Clase para leer las características del audio extraidas por el programa de Python
   */

    string rutaBeats = "Assets/Txt/beats.txt";
    string rutaSC = "Assets/Txt/scopt.txt"; 

    List<float> beats = new List<float>();
    List<float> scopt = new List<float>();

    void Start()
    {
        readBeats(rutaBeats);
        readSpectralCentroidOpt(rutaSC);
    }

    private void readBeats(string ruta)
    {
        // Lectura por líneas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);

            foreach (string line in lines)
                beats.Add(float.Parse(line) / 1000.0f);
        
        }
        else
            Debug.LogError("El archivo de texto para BEATS no existe en la ruta especificada.");
    }

    private void readSpectralCentroidOpt(string ruta)
    {
        // Lectura por líneas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);

            foreach (string line in lines)
            {
                //string[] words = line.Split(' ');
                //foreach (string word in words)
                //    beats.Add(float.Parse(line) / 1000.0f);
             
                scopt.Add(float.Parse(line) / 1000.0f);
            }
        }
        else
            Debug.LogError("El archivo de texto para SCOPT no existe en la ruta especificada.");
    }

    public List<float> getBeatsInTime()
    {
        return beats;
    }
    public List<float> getScopt()
    {
        return scopt;
    }
}