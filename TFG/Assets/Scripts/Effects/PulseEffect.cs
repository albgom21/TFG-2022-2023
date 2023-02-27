using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PulseEffect : MonoBehaviour
{
    public Image efecto;
    public ReadTxt input;

    private Color color;
    private float a;
    List<float> beats;
    int cont = 0;

    public float[] eventTimes;

    void Start()
    {
        Color color = efecto.color;
        beats = input.getBeatsInTime();

        foreach (float time in beats)
        {
            Invoke("TriggerEvent", time);
        }
    }

    void TriggerEvent()
    {
        color.a = 0.5f;
        efecto.color = color;
        //Debug.Log("Evento activado en " + Time.time + " segundos");
    }

    private void Update()
    {
        color.a -= 0.025f;
        efecto.color = color;
    }
}