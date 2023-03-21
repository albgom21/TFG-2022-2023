using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class ButtonFunc : MonoBehaviour
{
    public string extension;
    private string songName;

    public void PlayLvl()
    {
        songName = GetComponent<TextMeshProUGUI>().text;

        GameManager.instance.setSong(songName);
        GameManager.instance.setExtension(extension);

        //if (aun no han sido creados los txts)
            //pythonCall();

        SceneManager.LoadScene("SampleScene");
    }
    public void setExtension(string s)
    {
        extension = s;
    }

    private void pythonCall()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "python";
        startInfo.Arguments = "../Feature Extraction/librosa_ex.py " + songName + extension;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        UnityEngine.Debug.Log(output);
    }
}