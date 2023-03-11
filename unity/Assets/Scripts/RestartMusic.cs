using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartMusic : MonoBehaviour
{
    private SelectMusic audio;
    private void Start()
    {
        audio = GetComponent<SelectMusic>();
    }

    public void restartMusic()
    {
        audio.Stop();
        audio.Play();
    }
}
