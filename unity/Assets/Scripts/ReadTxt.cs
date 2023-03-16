using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[DefaultExecutionOrder(0)]
public class ReadTxt : MonoBehaviour
{
    /*
     Clase para leer las características del audio extraidas por el programa de Python
    */

    // RUTAS
    public string song = "200-BPM";         // Título del audio analizado
    string path = "Assets/Txt/";            // Ruta dentro del proyecto donde se guardan los txt
    string rutaBeats, rutaSC, rutaRMSE,     // Nombre de cada característica en los txt
           rutaSamples, rutaSr, rutaPlpBeats,
           rutaAgudosTiempo, rutaAgudosValoresNorm, rutaAgudos,
           rutaGravesTiempo, rutaGravesValoresNorm, rutaGraves,
           rutaOnset;


    // ESTRUCTURAS DE DATOS PARA GUARDAS LAS CARACTERÍSTICAS
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

    float[,] matriz_agudos;                     // Tiempo y valor en db de los agudos
    float[,] matriz_graves;                     // Tiempo y valor en db de los graves

    int sr;                                     // Sample rate (Frecuencia de muestreo)    

    void Awake()
    {
        GameManager.instance.setSong(song);
        GameManager.instance.setExtension(".wav");

        //song = GameManager.instance.getSong();

        // Crear las rutas de los txt
        rutaBeats = path + song + "_beats.txt";
        rutaPlpBeats = path + song + "_plpBeats.txt";
        rutaSC = path + song + "_scopt.txt";
        rutaRMSE = path + song + "_rmse.txt";
        rutaSamples = path + song + "_samples.txt";
        rutaSr = path + song + "_sr.txt";

        rutaAgudosTiempo = path + song + "_agudosTiempo.txt";
        rutaAgudosValoresNorm = path + song + "_agudosValorNorm.txt";
        rutaAgudos = path + song + "_agudos.txt";

        rutaGravesTiempo = path + song + "_gravesTiempo.txt";
        rutaGravesValoresNorm = path + song + "_gravesValorNorm.txt";
        rutaGraves = path + song + "_graves.txt";

        rutaOnset = path + song + "_onsetDetection.txt";

        // Leer y almacenar las caracteristicas del audio
        readFeature(ref beats, rutaBeats);
        //readFeature(ref plpBeats, rutaPlpBeats);
        readFeature(ref scopt, rutaSC);
        readFeature(ref rmse, rutaRMSE);
        readFeature(ref samples, rutaSamples);
        readFeature(ref agudosTiempo, rutaAgudosTiempo);
        readFeature(ref agudosValoresNorm, rutaAgudosValoresNorm);

        readFeature(ref gravesTiempo, rutaGravesTiempo);
        readFeature(ref gravesValoresNorm, rutaGravesValoresNorm);
        readFeature(ref onset, rutaOnset);
        readInt(ref sr, rutaSr);

        //readMatriz(ref matriz_agudos, rutaAgudos);
        //readMatriz(ref matriz_graves, rutaGraves);
    }

    // Lee una caracterísitca de audio que este en un txt, con una valor float por fila
    private void readFeature(ref List<float> lista, string ruta)
    {
        // Lectura por líneas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);
            foreach (string line in lines)
                lista.Add(float.Parse(line) / 1000.0f);
        }
        else Debug.LogError("El archivo de texto para leer una FEATURE no existe en la ruta especificada: " + ruta);
    }

    // Lee una caracterísitca de audio que este en un txt, siendo esta un único int
    private void readInt(ref int n, string ruta)
    {
        // Lectura por líneas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);
            n = int.Parse(lines[0]);
        }
        else Debug.LogError("El archivo de texto para leer un INT no existe en la ruta especificada: " + ruta);
    }

    // Lee una caracterísitca de audio que este en un txt, con la forma de una matriz bidimensional,
    // las filas compuestas por un valor float separado por un espacio seguido del otro valor float
    // el cambio de fila viene dado por cambio de linea \n
    private void readMatriz(ref float[,] matriz, string ruta)
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
                    if (numeros.Length != columnas) // Línea vacía o incompleta, salta esa iteración del bucle
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
    public List<float> getBeatsInTime()
    {
        return beats;
    }
    public List<float> getPlpBeatsInTime()
    {
        return plpBeats;
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
    public List<float> getAgudosTiempo()
    {
        return agudosTiempo;
    }
    public List<float> getGravesTiempo()
    {
        return gravesTiempo;
    }
    public List<float> getAgudosValoresNorm()
    {
        return agudosValoresNorm;
    }
    public List<float> getGravesValoresNorm()
    {
        return gravesValoresNorm;
    }
    public List<float> getOnset()
    {
        return onset;
    }
    public int getSr()
    {
        return sr;
    }
    public float[,] getAgudos()
    {
        return matriz_agudos;
    }
    public float[,] getGraves()
    {
        return matriz_graves;
    }
}