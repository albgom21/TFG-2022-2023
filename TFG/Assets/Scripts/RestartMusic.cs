using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartMusic : MonoBehaviour
{
    private FMODUnity.StudioEventEmitter audio;
    private void Start()
    {
        audio = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    public void restartMusic()
    {
        audio.Stop();
        audio.Play();
    }
}
