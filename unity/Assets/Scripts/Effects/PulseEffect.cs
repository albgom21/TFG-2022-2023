using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Cambiar la zona interior del player al ritmo de los beats
public class PulseEffect : MonoBehaviour
{
    public SpriteRenderer sprite;

    private float timeCount;
    private Color color;
    List<float> beats;
    int cont = 0;

    private void Awake()
    {
        GameManager.instance.SetPulseEffect(this);
    }

    void Start()
    {
        color = sprite.color;
        beats = GameManager.instance.GetFeatureManager().GetBeatsInTime();
        timeCount = -Constants.DELAY_TIME;
    }

    private void Update()
    {
        if (GameManager.instance.GetEnd()) return;

        timeCount += Time.deltaTime;

        if (cont < beats.Count() && timeCount >= beats[cont])
        {
            color.a = 1f;
            sprite.color = color;
            cont++;
        }

        color.a -= 1.5f * Time.deltaTime;
        sprite.color = color;
    }


    public void SyncroAfterPlayerDeath()
    {
        timeCount = (float)GameManager.instance.GetDeathTime() - Constants.DELAY_TIME;
        cont = GameManager.instance.GetLastBeatBeforeDeath();
    }
}
