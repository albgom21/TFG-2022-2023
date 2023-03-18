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
    public string[] extension;

    void Start()
    {
        // Obtener los nombres de las canciones de la carpeta "StreamingAssets"
        string[] filePaths = Directory.GetFiles(Application.dataPath + "/StreamingAssets", "*.mp3");
        filePaths = filePaths.Concat(Directory.GetFiles(Application.dataPath + "/StreamingAssets", "*.wav")).ToArray();
        filePaths = filePaths.Concat(Directory.GetFiles(Application.dataPath + "/StreamingAssets", "*.aif")).ToArray();
        filePaths = filePaths.Concat(Directory.GetFiles(Application.dataPath + "/StreamingAssets", "*.wma")).ToArray();
        filePaths = filePaths.Concat(Directory.GetFiles(Application.dataPath + "/StreamingAssets", "*.flac")).ToArray();
        filePaths = filePaths.Concat(Directory.GetFiles(Application.dataPath + "/StreamingAssets", "*.ogg")).ToArray();

        // Obtener solo los nombres de archivo sin la ruta y la extensión
        songNames = new string[filePaths.Length];
        extension = new string[filePaths.Length];
        for (int i = 0; i < filePaths.Length; i++)
        {
            songNames[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
            extension[i] = Path.GetExtension(filePaths[i]);
        }

        // Crear un botón por cada canción
        for (int i = 0; i < songNames.Length; i++)
        {
            // Crear un nuevo objeto de botón utilizando el prefab
            GameObject buttonObject = Instantiate(buttonPrefab, buttonContainer);
            // Asignar el nombre de la canción al botón
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = songNames[i];
            buttonObject.GetComponentInChildren<ButtonFunc>().setExtension(extension[i]);
        }
    }
}
