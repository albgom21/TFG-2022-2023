using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject features;
    [SerializeField] private Transform contenedor;
    [SerializeField] private int multiplierY;
    private float multiplierX;

    //public int BPM;
    //public float finalLevel;

    void Start()
    {
        float width = transform.localScale.x;
        float height = transform.localScale.y;
        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        List<float> scopt = features.GetComponent<ReadTxt>().getScopt();

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getPlayerSpeed();

        for (int i = 0; i < beats.Count(); i++)
        {
            float x = beats[i] * multiplierX;
            float y = scopt[i] * multiplierY;

            if (y > 0 && i > 0)
            {
                int distance = (int)(beats[i] * multiplierX - beats[i - 1] * multiplierX);
                int count = distance / (int)width;
                for (int j = 0; j < count; j++)
                    Instantiate(groundPrefab, new Vector3(x - width * j, y - height, 0), transform.rotation);
            }
            else if (i == 0)
            {
                for (int j = 0; j < 10; j++)
                    Instantiate(groundPrefab, new Vector3(x - width * j, y - height, 0), transform.rotation);
            }
            Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, contenedor);
        }
        //float distance = (60.0f / BPM) * 10.0f; //Distancia entre bloques = 60 / BPM (segundos en 1 minuto) * 10 (el player recorre 10 unidades por segundo)
        //for (float i = distance; i < finalLevel; i += distance)
        //{
        //    Instantiate(obstaclePrefab, new Vector3(i, 0, 0), transform.rotation);
        //}
    }

    public int getMultiplierX()
    {
        return multiplierX;
    }

    public GameObject getFeatures()
    {
        return features;
    }
}