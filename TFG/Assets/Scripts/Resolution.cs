using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resolution : MonoBehaviour
{
    public enum resLvl{Low = 0, Medium = 1, High = 2}
    public resLvl res = resLvl.High;
    public RenderTexture[] renderTexts;
    Camera cam;
    int i;
    void Start()
    {        
        cam = gameObject.GetComponent<Camera>();
        if (cam.targetTexture != null)        
            cam.targetTexture.Release();
        i = Convert.ToInt32(res);       
        cam.targetTexture = renderTexts[i];
    }
    private void Update()
    {
        if (Input.GetMouseButton(1)) {
            i++;
            cam.targetTexture = renderTexts[i%3];
        }
    }
}