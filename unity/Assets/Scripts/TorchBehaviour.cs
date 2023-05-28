using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchBehaviour : MonoBehaviour
{
    public SpriteRenderer sprite;
    public ReadTxt input;

    private float timeCount;
    private Light2D light;
    List<float> beats;
    int cont = 0;

    void Start()
    {
        input = GameManager.instance.GetFeatureManager();
        light = gameObject.GetComponent<Light2D>();
        beats = input.GetBeatsInTime();
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

    public void TorchSyncro()
    {
        Debug.Log("SINCRO");
        timeCount = (float)GameManager.instance.GetDeathTime() - Constants.DELAY_TIME;
        cont = GameManager.instance.GetLastBeatBeforeDeath();
    }
}
