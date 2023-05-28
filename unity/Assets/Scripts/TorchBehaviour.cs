using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchBehaviour : MonoBehaviour
{
    private float timeCount;
    private Light2D light;
    List<float> beats;
    int cont = 0;

    private void Awake()
    {
        GameManager.instance.AddTorch(this);
    }

    void Start()
    {
        light = gameObject.GetComponent<Light2D>();
        beats = GameManager.instance.GetFeatureManager().GetBeatsInTime();
        timeCount = -Constants.DELAY_TIME;
    }

    private void Update()
    {
        if (GameManager.instance.GetEnd()) return;

        timeCount += Time.deltaTime;

        if (cont < beats.Count && timeCount >= beats[cont])
        {
            light.intensity = 1f;
            cont++;
        }

        light.intensity -= 1.5f * Time.deltaTime;
    }

    public void SyncroAfterPlayerDeath()
    {
        timeCount = (float)GameManager.instance.GetDeathTime() - Constants.DELAY_TIME;
        cont = GameManager.instance.GetLastBeatBeforeDeath();
    }
}
