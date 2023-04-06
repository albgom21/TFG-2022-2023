using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Linq;
using UnityEngine.UI;

public class ButtonFunc : MonoBehaviour
{
    private string songName, extension;
    private string path, checkPath;
    List<string> pathsSpleeter, pathsFeatures;

    public void PlayLvl()
    {
        // Activar imagen de carga
        GameObject.FindGameObjectWithTag("LoadingImage").GetComponent<Image>().enabled = true;

        // Obtener componentes
        songName = GetComponent<TextMeshProUGUI>().text;

        // Establecer la canción y su extensión en el GM
        GameManager.instance.SetSong(songName);
        GameManager.instance.SetExtension(extension);

        // Rutas para leer las canciones y para comprobar ficheros generados
        creatrePaths();

        // Llamada a spleeter con 1er arg la canción y 2do arg el lugar donde deja las pistas generadas
        if (!checkFiles(pathsSpleeter))
            RunPythonScript("Assets/FeaturesExtraction/spleeter_ex.py", "./" + path, "./Assets/StreamingAssets/");

        // Si no existen los ficheros llamar a Python para la extracción de las características
        if (!checkFiles(pathsFeatures))
            RunPythonScript("Assets/FeaturesExtraction/librosa_ex.py", path);

        // Cargar escena del juego
        SceneManager.LoadScene("SampleScene");
    }

    // Crea las rutas necesarias para comprobar si ya existen los archivos y no ejecutar de nuevo los scripts de Python
    //----- AÑADIR MAS RUTAS ----
    private void creatrePaths()
    {
        path = "Assets/StreamingAssets/" + songName + extension;  // Ruta donde se encuentra la canción seleccionada
        checkPath = "Assets/FeaturesExtraction/Txt/" + songName;  // Ruta donde se encuentran las características de la canción

        pathsSpleeter = new string[] {                            // Lista de rutas que se esperan haber generado una vez se haya ejecutado spleeter
            "Assets/StreamingAssets/" + songName + "_drums" + extension // Audio de las baterías de la canción
        }.ToList();

        pathsFeatures = new string[]{   // Lista de rutas que se esperan haber generado una vez se haya ejecutado la extracción de características
            checkPath + "_beats.txt",
            checkPath + "_scopt.txt",
            checkPath + "_rmse.txt",
            checkPath + "_sr.txt",
            checkPath + "_duration.txt",
            checkPath + "_onsetDetection.txt" 
        }.ToList();
    }

    // Ejecuta el archivo python que se encuentra en filePath con los argumentos arguments
    // En caso de error escribe en consola el error capturado así como la salida de consola del script
    public void RunPythonScript(string filePath, string argument0, string argument1 = "")
    {
        string pythonExecutablePath = "python";

        // Comprobar que exista el archivo.py
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogError("El archivo de script de Python no existe en la ruta especificada.");
            return;
        }

        // Configuración del proceso
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonExecutablePath,
            Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\"", filePath, argument0, argument1),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        // Crear proceso con la configuración anterior
        Process process = new Process();
        process.StartInfo = startInfo;

        // Depurar salida y errores
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (sender, e) => { if (e.Data != null || e.Data != "") UnityEngine.Debug.Log(e.Data); };
        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) UnityEngine.Debug.LogError(e.Data); };

        // Comienzo del proceso
        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Esperar a que termine el proceso
        process.WaitForExit();

        if (process.ExitCode != 0)
            UnityEngine.Debug.LogErrorFormat("El script de Python falló con el código de salida {0}.", process.ExitCode);
    }

    // Comprueba si existen todos los ficheros de una lista de rutas
    // True -> existen todos los ficheros de la lista
    // False -> al menos un fichero de la lista no existe
    private bool checkFiles(List<string> paths) 
    {
        // Comprobar si existen todos los ficheros
        foreach (string s in paths)
            if (!File.Exists(s))
            {
                UnityEngine.Debug.Log("NO ESTA GENERADO EL FICHERO" + s);
                return false;
            }

        UnityEngine.Debug.Log("ESTAN TODOS LOS FICHEROS GENERADOS");

        return true;
    }

    public void setExtension(string s)
    {
        extension = s;
    }
}