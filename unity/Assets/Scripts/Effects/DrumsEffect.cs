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
    private Image image;
    private Color color;
    List<float> onset;
    private int i;
    private float offset;
    private float time;
    private int onsetCount;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
        color = image.color;
        onset = input.getOnset();
        i = 0;
        onsetCount = onset.Count;
    }

    private void Update()
    {
        if (!GameManager.instance.getEnd())
        {
            if (GameManager.instance.getDeath()) time = i = 0;
            time += Time.deltaTime;

            if (i < onsetCount && time >= onset[i])
            {
                color.a = 1f;
                if (i < onset.Count - 1)
                {
                    if (onset[i + 1] - onset[i] > 1f) offset = 1f;
                    else offset = onset[i + 1] - onset[i];
                }
                else offset = 1;
                i++;
            }
            image.color = color;
            color.a -= 1f * (Time.deltaTime / offset);
        }
    }
}
