using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartMusic : MonoBehaviour
{
    private new SelectMusic audio;
    private void Start()
    {
        audio = GetComponent<SelectMusic>();
    }

    public void restartMusic(int t)
    {
        //audio.Stop();
        audio.playTime(t);
        //audio.Play();
    }
    public void stopMusic()
    {
        audio.Stop();
    }
}