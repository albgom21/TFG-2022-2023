using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rmseTests : MonoBehaviour
{
    public ReadTxt samples;
    List<float> rmse;
    List<float> beats;
    int cont = 0;
    void Start()
    {
        rmse = samples.getRMSE();
        beats = samples.getBeatsInTime();
        foreach (float b in beats)
            Invoke("rmseSize", b);

    }
    void rmseSize()
    {
        transform.localScale = new Vector3(rmse[cont] * 10, 1, 1);
        cont++;
    }
}
