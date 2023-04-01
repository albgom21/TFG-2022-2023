using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SpleeterAuto : MonoBehaviour
{
    public void commandCall(string path)
    {
        // UnityEngine.Debug.Log("LLAMA A SPLEETER en " + path);
        // // Define la ruta de la carpeta donde se ejecutar� el comando (ruta donde est� spleeter)
        // //string workingDirectory = "C:\\ruta\\a\\la\\carpeta";

        // // Define el comando a ejecutar
        // string command = "spleeter separate -p spleeter:2stems -o Assets/StreamingAssets/ " + path;

        // // Crea un proceso para ejecutar el comando
        // Process process = new Process();
        // process.StartInfo.FileName = "cmd.exe"; // Utiliza la consola de Windows para ejecutar el comando
        // //process.StartInfo.WorkingDirectory = workingDirectory; // Establece la carpeta de trabajo del proceso
        // process.StartInfo.Arguments = "/C " + command; // Indica el comando que se va a ejecutar
        // process.StartInfo.UseShellExecute = false; // No utiliza la shell de Windows
        // process.StartInfo.RedirectStandardOutput = true; // Redirige la salida est�ndar a una variable
        // process.Start();

        // // Lee la salida del comando y la muestra en la consola de Unity
        // string output = process.StandardOutput.ReadToEnd();
        // UnityEngine.Debug.Log(output);

        // // Espera a que el proceso termine
        // process.WaitForExit();
    }
}