using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Linq;

public class ButtonFunc : MonoBehaviour
{
    private string extension;
    private string songName;
    private string path, checkPath;
    private SpleeterAuto spleeter;

    public void PlayLvl()
    {
        songName = GetComponent<TextMeshProUGUI>().text;
        spleeter = GetComponent<SpleeterAuto>();

        GameManager.instance.setSong(songName);
        GameManager.instance.setExtension(extension);

        path = "Assets/StreamingAssets/" + songName + extension;
        checkPath = "Assets/FeaturesExtraction/Txt/" + songName;

        spleeter.commandCall(path);

        // Check si existen ya los ficheros
        if (!checkFiles()) 
            pythonCall();

        SceneManager.LoadScene("SampleScene");
    }
    public void setExtension(string s)
    {
        extension = s;
    }

    private void pythonCall()
    {
        string arguments = "Assets/FeaturesExtraction/librosa_ex.py " + path;
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "python";
        startInfo.Arguments = arguments;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        //UnityEngine.Debug.Log("Resultado: " + output);
    }
    private bool checkFiles() // REFACTOR DE TODAS LAS RUTAS - AÑADIR MÄS RUTAS
    {
        List<string> rutas = new string[]{checkPath + "_beats.txt", checkPath + "_scopt.txt" , checkPath + "_rmse.txt", checkPath + "_sr.txt",
        checkPath + "_duration.txt"/*, checkPath + "_onsetDetection.txt"*/ }.ToList();

        foreach (string s in rutas)
            if (!File.Exists(s))
            {
                UnityEngine.Debug.Log("NO ESTA " + s);
                return false;
            }
        UnityEngine.Debug.Log("ESTAN TODAS LAS RUTAS");

        return true;
    }
}