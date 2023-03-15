using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class DrumsEffect : MonoBehaviour
{
    public ReadTxt input;
    private Image image;
    private Color color;
    List<float> onset;
    private int i;
    private float offset;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
        color = image.color;
        onset = input.getOnset();
        i = 0;
        foreach (float time in onset)
        {
            Invoke("DrumsEvent", time);
        }

    }

    void DrumsEvent()
    {
        color.a = 1f;
        if (i < onset.Count - 1)
        {
            if (onset[i + 1] - onset[i] > 1f) offset = 1f;
            else offset = onset[i + 1] - onset[i];
            Debug.Log(offset);
        }
        else offset = 1;
        i++;
    }

    private void Update()
    {
        color.a -= 1f * (Time.deltaTime/offset);
        image.color = color;
    }
}
