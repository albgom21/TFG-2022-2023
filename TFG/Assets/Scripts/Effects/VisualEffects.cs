using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualEffects : MonoBehaviour
{
    public Material skyboxNew;
    private Material skyboxOld;

    void Start()
    {
        skyboxOld = RenderSettings.skybox;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            RenderSettings.skybox = skyboxNew;
            transform.localRotation =  new Quaternion(-180,0,0,0);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            RenderSettings.skybox = skyboxOld;
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

    }
}