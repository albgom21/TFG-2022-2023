using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionRawImage : MonoBehaviour
{
    public enum resLvl { Low = 0, Medium = 1, High = 2 }
    resLvl res = resLvl.High;
    public RenderTexture[] renderTexts;
    RawImage raw;
    int i;

    void Start()
    {
        raw = gameObject.GetComponent<RawImage>();        
        i = Convert.ToInt32(res);
        raw.texture = renderTexts[i];
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    i++;
        //    raw.texture = renderTexts[i%3];
        //}
    }
}
