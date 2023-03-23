using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PulseEffect : MonoBehaviour
{
    //public Image efecto;
    public SpriteRenderer sprite;
    public ReadTxt input;

    private float timeCount = 0;
    private Color color;
    List<float> beats;
    int cont = 0;

    void Start()
    {
        color = sprite.color;
        beats = input.getBeatsInTime();
        //beats = input.getPlpBeatsInTime();

        //foreach (float time in beats)
        //{
        //    Invoke("TriggerEvent", time);
        //}
    }

    //void TriggerEvent()
    //{
    //    color.a = 0.5f;
    //    efecto.color = color;
    //    //Debug.Log("Evento activado en " + Time.time + " segundos");
    //}

    private void Update()
    {
        if (!GameManager.instance.getEnd())
        {

            if (GameManager.instance.getDeath())
            {
                timeCount = 0;
                cont = 0;
            }

            timeCount += Time.deltaTime;

            if (cont < beats.Count() && timeCount >= beats[cont])
            {
                color.a = 1f;
                sprite.color = color;
                cont++;
            }

            color.a -= 1f * Time.deltaTime;
            sprite.color = color;
        }
    }
}