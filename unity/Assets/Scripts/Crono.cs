using System;
using UnityEngine;
using UnityEngine.UI;

// Clase para mostrar el tiempo actual de la canci�n
public class Crono : MonoBehaviour
{
    public Text textoCrono;

    private double timeActivated = 0;
    private double t = 0;

    private void Start()
    {
        timeActivated = Time.time;
    }

    private void Update()
    {
        t = Time.time - timeActivated;                // Calcular el tiempo que lleva el crono activado         
        textoCrono.text = t.ToString("F2") + "s";        // Mostrar el tiempo sin decimales   
    }
}