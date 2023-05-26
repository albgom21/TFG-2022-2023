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

    public void ResetMusic(int t)
    {
        audio.PlayTime(t);
    }

    public void StopMusic()
    {
        audio.Stop();
    }

    public void ResumeMusic()
    {
        audio.Resume();
    }
}