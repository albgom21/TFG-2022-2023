using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ReadTxt : MonoBehaviour
{
    /*
     Clase para leer las características del audio extraidas por el programa de Python
    */
    public string song = "200-BPM";
    string path= "Assets/Txt/";
    string rutaBeats,rutaSC, rutaRMSE, rutaSamples, rutaSr;

    List<float> beats = new List<float>();
    List<float> scopt = new List<float>();
    List<float> rmse = new List<float>();
    List<float> samples = new List<float>();
    int sr;

    void Awake()
    {
        rutaBeats = path + song + "_beats.txt";
        rutaSC = path + song + "_scopt.txt";
        rutaRMSE = path + song + "_rmse.txt";
        rutaSamples = path + song + "_samples.txt";
        rutaSr = path + song + "_sr.txt";

        readBeats(rutaBeats);
        readSpectralCentroidOpt(rutaSC);
        readRMSE(rutaRMSE);
        readSamples(rutaSamples);
        readSr(rutaSr);
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

    private void readRMSE(string ruta)
    {
        // Lectura por líneas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);

            foreach (string line in lines)
                rmse.Add(float.Parse(line) / 1000.0f);

        }
        else
            Debug.LogError("El archivo de texto para RMSE no existe en la ruta especificada.");
    }

    private void readSamples(string ruta)
    {
        // Lectura por líneas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);

            foreach (string line in lines)
                samples.Add(float.Parse(line) / 1000.0f);

        }
        else
            Debug.LogError("El archivo de texto para SAMPLES no existe en la ruta especificada.");
    }

    private void readSr(string ruta)
    {
        // Lectura por líneas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);

            sr = int.Parse(lines[0]);
        }
        else
            Debug.LogError("El archivo de texto para SR no existe en la ruta especificada.");
    }

    public List<float> getBeatsInTime()
    {
        return beats;
    }
    public List<float> getScopt()
    {
        return scopt;
    }
    public List<float> getRMSE()
    {
        return rmse;
    }
    public List<float> getSamples()
    {
        return samples;
    }
    public int getSr()
    {
        return sr;
    }
}