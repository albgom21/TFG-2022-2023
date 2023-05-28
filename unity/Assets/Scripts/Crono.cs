using System;
using UnityEngine;
using UnityEngine.UI;

// Clase para mostrar el tiempo actual de la canción
public class Crono : MonoBehaviour
{
    private Text textoCrono;
    private DebugManager debugManager;

    private double timeActivated = 0;   // Tiempo en el que ha activado el crono
    private double t = 0;               // Tiempo real de crono activo  
    private double spawnTime = 0;       // Tiempo en el que se produjo el último spawn

    private void Awake()
    {
        GameManager.instance.SetCrono(this);
    }

    private void Start()
    {
        textoCrono = GetComponent<Text>();
        timeActivated = Time.time;

        debugManager = GameManager.instance.GetDebugManager();
        debugManager.setCronoInstance(textoCrono);
        textoCrono.enabled = debugManager.getDebugMode();
    }

    private void Update()
    {
        if (GameManager.instance.GetEnd()) return;
        t = Time.time - timeActivated + spawnTime;       // Calcular el tiempo que lleva el crono activado         
        textoCrono.text = t.ToString("F2") + "s";        // Mostrar el tiempo sin decimales   
    }

    public double getActualTime() { return t; }
    public void setActualTime(double t_)
    {
        spawnTime = t_;
    }

    public void SyncroAfterPlayerDeath() { timeActivated = Time.time; }
}