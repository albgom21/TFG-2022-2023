using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{

    public GameObject obstaclePrefab;
    public GameObject features;
    public Transform contenedor;

    //public int BPM;
    //public float finalLevel;

    void Start()
    {
        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        List<float> scopt = features.GetComponent<ReadTxt>().getScopt();

        for (int i = 0; i < beats.Count(); i++)
        {
            float x = beats[i] * 15;
            float y = scopt[i] * 10;
            Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, contenedor);
        }
        //float distance = (60.0f / BPM) * 10.0f; //Distancia entre bloques = 60 / BPM (segundos en 1 minuto) * 10 (el player recorre 10 unidades por segundo)
        //for (float i = distance; i < finalLevel; i += distance)
        //{
        //    Instantiate(obstaclePrefab, new Vector3(i, 0, 0), transform.rotation);
        //}
    }
}