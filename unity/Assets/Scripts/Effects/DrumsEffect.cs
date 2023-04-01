using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DrumsEffect : MonoBehaviour
{
    public ReadTxt input;
    private Light2D light_;
    [SerializeField] private float intensity;
    List<float> onset;
    private int i, maxIntensity,onsetCount;
    private float offset, time;


    void Start()
    {
        light_ = gameObject.GetComponent<Light2D>();
        onset = input.getOnset();
        intensity = i = 9;
        maxIntensity = 1;
        onsetCount = onset.Count;
    }

    private void Update()
    {
        light_.intensity = intensity;
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
        light_.intensity = intensity;
        intensity -= maxIntensity * (Time.deltaTime / offset);
    }
}
