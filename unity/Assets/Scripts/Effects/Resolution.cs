using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resolution : MonoBehaviour
{
    [SerializeField]
    private RenderTexture lowRes;
    [SerializeField]
    private GameObject rawImage;

    private Camera cam;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        if (cam.targetTexture != null)
            cam.targetTexture.Release();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cam.targetTexture = lowRes;
            rawImage.SetActive(true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            cam.targetTexture = null;
            rawImage.SetActive(false);
        }
    }
}