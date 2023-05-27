using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMusic : MonoBehaviour
{
    private new MainMusic audio;

    private void Awake()
    {
        audio = GetComponent<MainMusic>();
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

    public void SetVolume(float v)
    {
        audio.SetVolume(v);
    }
}