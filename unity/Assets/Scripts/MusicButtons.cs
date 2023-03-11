using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MusicButtons : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public string[] songNames;

    void Start()
    {
        // Obtener los nombres de las canciones de la carpeta "Songs"
        string[] filePaths = Directory.GetFiles(Application.dataPath + "/StreamingAssets", "*.mp3");
        filePaths = filePaths.Concat(Directory.GetFiles(Application.dataPath + "/StreamingAssets", "*.wav")).ToArray();

        // Obtener solo los nombres de archivo sin la ruta y la extensi�n
        songNames = new string[filePaths.Length];
        for (int i = 0; i < filePaths.Length; i++)
        {
            songNames[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
        }


        // Crear un bot�n por cada canci�n
        foreach (string songName in songNames)
        {
            // Crear un nuevo objeto de bot�n utilizando el prefab
            GameObject buttonObject = Instantiate(buttonPrefab, buttonContainer);

            // Asignar el nombre de la canci�n al bot�n
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = songName;
        }
    }
}
