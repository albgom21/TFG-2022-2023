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
    private string songPath, checkPath, extractionPath;
    List<string> pathsSpleeter, pathsFeatures;
    
    public Image img;

    [SerializeField]
    private Color deleteInactive;
    public void PlayLvl()
    {
        // Sonido clic
        FMODUnity.RuntimeManager.PlayOneShot("event:/MenuSelection");

        // Activar imagen de carga
        GameObject.FindGameObjectWithTag("LoadingImage").GetComponent<Image>().enabled = true;

        // Obtener componentes
        songName = GetComponent<TextMeshProUGUI>().text;

        // Establecer la canción y su extensión en el GM
        GameManager.instance.SetSong(songName);
        GameManager.instance.SetExtension(extension);

        // Rutas para leer las canciones y para comprobar ficheros generados
        createPaths();

        // Llamada a spleeter con 1er arg la canción y 2do arg el lugar donde deja las pistas generadas
        if (!checkFiles(pathsSpleeter))
            RunPythonScript(Application.streamingAssetsPath + "/FeaturesExtraction/Python/spleeter_ex.py", songPath, extractionPath);

        // Si no existen los ficheros llamar a Python para la extracción de las características
        if (!checkFiles(pathsFeatures))
        {
            RunPythonScript(Application.streamingAssetsPath + "/FeaturesExtraction/Python/librosa_ex.py", songPath);
            moveTxts(Application.streamingAssetsPath + "/", extractionPath);
        }

        // Cargar escena del juego
        SceneManager.LoadScene(Constants.NAME_GAME_SCENE);
    }

    // Crea las rutas necesarias para comprobar si ya existen los archivos y no ejecutar de nuevo los scripts de Python
    //----- AÑADIR MAS RUTAS ----
    private void createPaths()
    {
        songPath = Application.streamingAssetsPath + "/" + songName + extension;     // Ruta donde se encuentra la canción seleccionada

        extractionPath = Application.streamingAssetsPath + "/" + songName + "/";     // Ruta donde se encuentran los txt de la cancion

        checkPath = extractionPath + songName;  // Ruta para formar los txt necesarios de características de la canción

        pathsSpleeter = new string[] {                            // Lista de rutas que se esperan haber generado una vez se haya ejecutado spleeter
            extractionPath + songName + "_drums.wav"// Audio de las baterías de la canción
        }.ToList();

        pathsFeatures = new string[]{   // Lista de rutas que se esperan haber generado una vez se haya ejecutado la extracción de características
            checkPath + "_beats.txt",
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
                UnityEngine.Debug.Log("NO ESTA GENERADO EL FICHERO: " + s);
                return false;
            }

        UnityEngine.Debug.Log("ESTAN TODOS LOS FICHEROS GENERADOS");

        return true;
    }

    private void moveTxts(string pathOrg, string pathDst)
    {
        // Obtener la lista de archivos de la carpeta de origen que tienen extensión .txt
        string[] archivos = Directory.GetFiles(pathOrg, "*.txt");

        foreach (string archivo in archivos)
        {
            // Construir la ruta completa de la carpeta de destino, manteniendo el nombre del archivo
            string archivoDestino = Path.Combine(pathDst, Path.GetFileName(archivo));

            // Mover el archivo desde la carpeta de origen a la carpeta de destino
            File.Move(archivo, archivoDestino);
        }
    }

    public void deleteLvl()
    {
        // Sonido clic
        FMODUnity.RuntimeManager.PlayOneShot("event:/MenuSelection");

        songName = GetComponent<TextMeshProUGUI>().text;

        string filePath = Application.streamingAssetsPath + "/" + songName + "/" + songName + "_levelInfo.txt";
        if (File.Exists(filePath))
        {
            // Si existe el fichero de guardado cambiar de color el boton y borrar el fichero
            img.color = deleteInactive;
            File.Delete(filePath);
        }
    }

    public void setExtension(string s)
    {
        extension = s;
    }
}