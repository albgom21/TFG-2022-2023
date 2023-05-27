using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumenBar : MonoBehaviour
{
    public ControlMusic music;
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();

        if (PlayerPrefs.HasKey("Volumen"))
            slider.value = PlayerPrefs.GetFloat("Volumen");
    }

    public void ChangeValue(float value)
    {
        music.SetVolume(value);
        PlayerPrefs.SetFloat("Volumen", value);
        PlayerPrefs.Save();
    }
}