using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider slider;

    [SerializeField]
    private Crono crono;

    void Start()
    {
        slider = GetComponent<Slider>();
        changeSongDuration(147); //Poner la duración verdadera de la canción.
    }

    public void changeSongDuration(float duration)
    {
        slider.maxValue = duration;
    }
    public void changeProgress(float duration)
    {
        slider.value = duration;
    }

    void Update()
    {
        changeProgress((float)crono.getActualTime());
    }
}