using System;
using UnityEngine;
using UnityEngine.UI;

// Clase para mostrar el tiempo actual de la canción
public class Crono : MonoBehaviour
{
    public Text textoCrono;

    private double timeActivated = 0;
    private double t = 0;
    private double spawnTime = 0;

    private void Start()
    {
        timeActivated = Time.time;
    }

    private void Update()
    {
        if (!GameManager.instance.getEnd())
        {
            if (GameManager.instance.getDeath())
                timeActivated = Time.time;
            t = Time.time - timeActivated + spawnTime;                   // Calcular el tiempo que lleva el crono activado         
            textoCrono.text = t.ToString("F2") + "s";        // Mostrar el tiempo sin decimales   
        }
    }

    public double getActualTime() { return t; }
    public void setActualTime (double t_) {
        spawnTime = t_;
    }
}