using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumenBar : MonoBehaviour
{
    public RestartMusic music;

    public void ChangeValue(float value)
    {
        music.SetVolume(value);
    }
}