using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[DefaultExecutionOrder(0)]
public class ReadTxt : MonoBehaviour
{
    /*
     Clase para leer las caracteristicas del audio extraidas por el programa de Python
    */

    // DEBUG
    public bool pruebasDesdeMenu = false;

    // RUTAS
    public string song = "200-BPM";         // Titulo del audio analizado
    public string format = ".wav";
    string path;                            // Ruta dentro del proyecto donde se guardan los txt
    string rutaBeats, rutaSC, rutaRMSE,     // Nombre de cada caracteristica en los txt
           rutaSamples, rutaSr, rutaDuration, rutaPlpBeats,
           rutaAgudosTiempo, rutaAgudosValoresNorm, rutaAgudos,
           rutaGravesTiempo, rutaGravesValoresNorm, rutaGraves,
           rutaOnset, rutaOnsetPiano, rutaOnsetOther; //LAS DOS ÚLTIMAS SON PROVISIONALES


    // ESTRUCTURAS DE DATOS PARA GUARDAS LAS CARACTERISTICAS
    List<float> beats = new List<float>();      // Tiempo en seg cuando se producen los beats
    List<float> plpBeats = new List<float>();
    List<float> scopt = new List<float>();      // Valor del centroide espectral en los instantes en los que hay beats
    List<float> rmse = new List<float>();       // Valor del RMSE en los instantes en los que hay beats
    List<float> samples = new List<float>();    // Valor de las muestras del audio
    List<float> agudosTiempo = new List<float>();
    List<float> gravesTiempo = new List<float>();
    List<float> agudosValoresNorm = new List<float>();
    List<float> gravesValoresNorm = new List<float>();
    List<float> onset = new List<float>();
    List<float> onset_piano = new List<float>(); //PROVISIONAL PARA PRUEBAS
    List<float> onset_other = new List<float>(); //PROVISIONAL PARA PRUEBAS

    float[,] matriz_agudos;                     // Tiempo y valor en db de los agudos
    float[,] matriz_graves;                     // Tiempo y valor en db de los graves

    int sr;                                     // Sample rate (Frecuencia de muestreo)    
    float duration;                             // Duraci�n en segundos del audio

    void Awake()
    {
        //Desde la escena del propio nivel
        if (!pruebasDesdeMenu)
        {
            GameManager.instance.SetSong(song);
            GameManager.instance.SetExtension(format);
        }
        else   //Desde el menu
            song = GameManager.instance.GetSong();

        path = Application.streamingAssetsPath + "/" + song + "/"; //Ruta donde se encuentran los txt con las caract
        //Debug.Log("Path: " + path);
        // Crear las rutas de los txt
        rutaBeats = path + song + "_beats.txt";
        rutaPlpBeats = path + song + "_plpBeats.txt";
        rutaSC = path + song + "_scopt.txt";
        rutaRMSE = path + song + "_rmse.txt";
        rutaSamples = path + song + "_samples.txt";
        rutaSr = path + song + "_sr.txt";
        rutaDuration = path + song + "_duration.txt";

        rutaAgudosTiempo = path + song + "_agudosTiempo.txt";
        rutaAgudosValoresNorm = path + song + "_agudosValorNorm.txt";
        rutaAgudos = path + song + "_agudos.txt";

        rutaGravesTiempo = path + song + "_gravesTiempo.txt";
        rutaGravesValoresNorm = path + song + "_gravesValorNorm.txt";
        rutaGraves = path + song + "_graves.txt";

        rutaOnset = path + song + "_onsetDetection.txt";
        //PROVISIONAL
        rutaOnsetPiano = path + song + "_onsetPiano.txt";
        rutaOnsetOther = path + song + "_onsetOther.txt";

        // Leer y almacenar las caracteristicas del audio
        ReadFeature(ref beats, rutaBeats);
        //readFeature(ref plpBeats, rutaPlpBeats);
        ReadFeature(ref scopt, rutaSC);
        ReadFeature(ref rmse, rutaRMSE);
        //readFeature(ref samples, rutaSamples);
        ReadFeature(ref agudosTiempo, rutaAgudosTiempo);
        ReadFeature(ref agudosValoresNorm, rutaAgudosValoresNorm);

        ReadFeature(ref gravesTiempo, rutaGravesTiempo);
        ReadFeature(ref gravesValoresNorm, rutaGravesValoresNorm);
        ReadFeature(ref onset, rutaOnset);
        ReadInt(ref sr, rutaSr);
        ReadFloat(ref duration, rutaDuration);

        ReadFeature(ref onset_piano, rutaOnsetPiano);
        ReadFeature(ref onset_other, rutaOnsetOther);

        //readMatriz(ref matriz_agudos, rutaAgudos);
        //readMatriz(ref matriz_graves, rutaGraves);
    }

    // Lee una caracterisitca de audio que este en un txt, con una valor float por fila
    private void ReadFeature(ref List<float> lista, string ruta)
    {
        // Lectura por lineas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);
            foreach (string line in lines)
                lista.Add(float.Parse(line) / 1000.0f);
        }
        else Debug.LogError("El archivo de texto para leer una FEATURE no existe en la ruta especificada: " + ruta);
    }

    // Lee una caracterisitca de audio que este en un txt, siendo esta un �nico int
    private void ReadInt(ref int n, string ruta)
    {
        // Lectura por lineas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);
            n = int.Parse(lines[0]);
        }
        else Debug.LogError("El archivo de texto para leer un INT no existe en la ruta especificada: " + ruta);
    }

    // Lee una caracterisitca de audio que este en un txt, siendo esta un unico float
    private void ReadFloat(ref float n, string ruta)
    {
        // Lectura por lineas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);
            n = float.Parse(lines[0]);
        }
        else Debug.LogError("El archivo de texto para leer un FLOAT no existe en la ruta especificada: " + ruta);
    }

    // Lee una caracterisitca de audio que este en un txt, con la forma de una matriz bidimensional,
    // las filas compuestas por un valor float separado por un espacio seguido del otro valor float
    // el cambio de fila viene dado por cambio de linea \n
    private void ReadMatriz(ref float[,] matriz, string ruta)
    {
        if (File.Exists(ruta))
        {
            string texto = File.ReadAllText(ruta);
            string[] lineas = texto.Split('\n');
            int filas = lineas.Length;
            int columnas = lineas[0].Split(' ').Length;
            matriz = new float[filas, columnas];

            for (int i = 0; i < filas; i++)
            {
                string[] numeros = lineas[i].Split(' ');
                for (int j = 0; j < columnas; j++)
                {
                    if (numeros.Length != columnas) // Linea vacia o incompleta, salta esa iteracion del bucle
                        continue;
                    matriz[i, j] = float.Parse(numeros[j]) / 1000.0f;
                }
            }
        }
        else
            Debug.LogError("El archivo de texto para leer una MATRIZ no existe en la ruta especificada: " + ruta);

        // Escribir los valores de la matriz
        //for (int i = 0; i < matriz.GetLength(0); i++)
        //    for (int j = 0; j < matriz.GetLength(1); j++)
        //        Debug.Log(matriz[i, j] + " ");
    }

    // Getters
    public List<float> GetBeatsInTime() { return beats; }
    public List<float> GetPlpBeatsInTime() { return plpBeats; }
    public List<float> GetScopt() { return scopt; }
    public List<float> GetRMSE() { return rmse; }
    public List<float> GetSamples() { return samples; }
    public List<float> GetAgudosTiempo() { return agudosTiempo; }
    public List<float> GetGravesTiempo() { return gravesTiempo; }
    public List<float> GetAgudosValoresNorm() { return agudosValoresNorm; }
    public List<float> GetGravesValoresNorm() { return gravesValoresNorm; }
    public List<float> GetOnset() { return onset; }
    public List<float> GetOnsetPiano() { return onset_piano; } //PROVISIONAL
    public List<float> GetOnsetOther() { return onset_other; } //PROVISIONAL
    public int GetSr() { return sr; }
    public float GetDuration() { return duration; }
    public float[,] GetAgudos() { return matriz_agudos; }
    public float[,] GetGraves() { return matriz_graves; }
}