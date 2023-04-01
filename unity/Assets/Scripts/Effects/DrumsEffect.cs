using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class DrumsEffect : MonoBehaviour
{
    public ReadTxt input;
    private Light light;
    private float intensity;
    List<float> onset;
    private int i, maxIntensity,onsetCount;
    private float offset, time;


    void Start()
    {
        light = gameObject.GetComponent<Light>();
        onset = input.getOnset();
        intensity = i = 9;
        maxIntensity = 10;
        onsetCount = onset.Count;
    }

    private void Update()
    {
        light.intensity = intensity;
        if (GameManager.instance.getEnd()) return;

        if (GameManager.instance.getDeath()) time = i = 0;
        time += Time.deltaTime;
        //Debug.Log(time + "  "+ onset[0]);

        if (i < onsetCount && time >= onset[i])
        {
            intensity = maxIntensity;
            if (i < onset.Count - 1)
            {
                if (onset[i + 1] - onset[i] > 1f) offset = 1f;
                else offset = onset[i + 1] - onset[i];
            }
            else offset = 1;
            i++;
        }
        light.intensity = intensity;
        intensity -= maxIntensity * (Time.deltaTime / offset);
    }
}
