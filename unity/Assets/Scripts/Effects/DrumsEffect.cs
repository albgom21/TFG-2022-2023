using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZoneCode;

public class DrumsEffect : MonoBehaviour
{
    public ReadTxt input;
    private Light2D light_;
    private float intensity, maxIntensity;
    List<float> onset;
    private int i, onsetCount;
    private double time;
    private float offset;

    void Start()
    {
        light_ = gameObject.GetComponent<Light2D>();
        onset = input.getOnset();
        i = 0;
        intensity = maxIntensity = light_.intensity;
        onsetCount = onset.Count;
        GameManager.instance.setDrumsEffect(this);
    }

    private void Update()
    {
        if (GameManager.instance.getEnd()) return;
        //if (GameManager.instance.getDeath())
        //{
        //    time = GameManager.instance.getDeathTime();
        //    i = 0;
        //    while (onset[i] < time) i++;
        //}

        time += Time.deltaTime;

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
        light_.intensity = intensity;
        intensity -= maxIntensity * (Time.deltaTime / offset);
    }

    public void SetLightColor(Color c) { light_.color = c; }
}
