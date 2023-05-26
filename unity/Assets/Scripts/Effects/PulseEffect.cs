using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Cambiar la zona interior del player al ritmo de los beats
public class PulseEffect : MonoBehaviour
{
    public SpriteRenderer sprite;
    public ReadTxt input;

    private float timeCount = 0;
    private Color color;
    List<float> beats;
    int cont = 0;

    void Start()
    {
        color = sprite.color;
        beats = input.GetBeatsInTime();
    }

    private void Update()
    {
        // !!! Meter lógica de los spawns
        if (!GameManager.instance.GetEnd())
        {
            if (GameManager.instance.GetDeath())
            {
                timeCount = (float) GameManager.instance.GetDeathTime() - Constants.DELAY_TIME;
                cont = GameManager.instance.GetLastBeatBeforeDeath();
            }

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
    }
}