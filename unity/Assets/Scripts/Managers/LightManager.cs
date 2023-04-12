using FMOD;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Debug = UnityEngine.Debug;

public class LightManager : MonoBehaviour
{
    [SerializeField] private ReadTxt input;
    [SerializeField] private Light2D backgroundLight;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    private float intensity, maxIntensity;
    List<float> onset;
    private int i, onsetCount;
    private double time;
    private float offset;
    private Color newBackgroundColor, newLightColor;
    
    void Start()
    {
        Debug.Log("LIGHT MANAGER START");
        onset = input.GetOnsetOther();
        for (int j = 0; j < onset.Count; j++)
        {
            onset[j] += Constants.DELAY_TIME;
            if (j == 0) Debug.Log(onset[j]);
        }

        i = 0;
        time = 0;
        intensity = maxIntensity = backgroundLight.intensity;
        onsetCount = onset.Count;
        newBackgroundColor = new Color(0, 0.64f, 1f, 0.3f);
        newLightColor = Color.blue;
        GameManager.instance.SetLightManager(this);
    }

    private void Update()
    {
        if (GameManager.instance.GetEnd()) return;
        if (GameManager.instance.GetDeath())
        {
            time = GameManager.instance.GetDeathTime();
            i = 0;
            while (onset[i] < time) i++;
        }

        time += Time.deltaTime;

        if (i < onsetCount && time >= onset[i])
        {
            intensity = maxIntensity;
            if (i < onset.Count - 1)
            {
                if (onset[i + 1] - onset[i] > 1f) offset = 1f;
                else offset = onset[i + 1] - onset[i];
            }
            else offset = 1;
            i++;
        }
        backgroundLight.intensity = intensity;
        intensity -= maxIntensity * (Time.deltaTime / offset);

        if (backgroundRenderer != null && backgroundRenderer.color != newBackgroundColor)
            backgroundRenderer.color = new Color(Mathf.Lerp(backgroundRenderer.color.r, newBackgroundColor.r, Time.deltaTime * 1.5f),
                Mathf.Lerp(backgroundRenderer.color.g, newBackgroundColor.g, Time.deltaTime * 1.5f),
                Mathf.Lerp(backgroundRenderer.color.b, newBackgroundColor.b, Time.deltaTime * 1.5f), backgroundRenderer.color.a);
        if (backgroundLight != null && backgroundLight.color != newLightColor)
        {
            backgroundLight.color = new Color(Mathf.Lerp(backgroundLight.color.r, newLightColor.r, Time.deltaTime * 1.5f),
                Mathf.Lerp(backgroundLight.color.g, newLightColor.g, Time.deltaTime * 1.5f),
                Mathf.Lerp(backgroundLight.color.b, newLightColor.b, Time.deltaTime * 1.5f), backgroundLight.color.a);
        }

    }

    public void SetLightColor(Color lightColor, Color backgroundColor)
    {
        newLightColor = lightColor;
        newBackgroundColor = backgroundColor;
    }
}
