using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadTxt : MonoBehaviour
{
    /*

    Clase para leer las características del audio extraidas por el programa de Python

   */

    public string ruta = "Assets/Txt/test.txt"; // Cambiar por la ruta correcta

    void Start()
    {
        // Lectura por líneas y luego palabras
        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);

            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                foreach (string word in words)
                  Debug.Log(word);
            }
        }
        else
            Debug.LogError("El archivo de texto no existe en la ruta especificada.");
    }
}


//CÓDIGO EN PYTHON PARA GUARDAR VARIABLES
/*
    # Crea una variable para guardar
    variable = 42

    # Abre un archivo de texto para escribir
    with open("archivo.txt", "w") as file:
        # Convierte la variable a una cadena de texto
        variable_string = str(variable)
        # Escribe la cadena de texto en el archivo
        file.write(variable_string)
 */
