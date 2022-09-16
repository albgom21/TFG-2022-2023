using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{

    public GameObject obstaclePrefab;
    public int BPM;
    public float finalLevel;

    // Start is called before the first frame update
    void Start()
    {
        float distance = (60.0f / BPM) * 10.0f; //Distancia entre bloques = 60 / BPM (segundos en 1 minuto) * 10 (el player recorre 10 unidades por segundo)

        for (float i = distance; i < finalLevel; i += distance)
        {
            Instantiate(obstaclePrefab, new Vector3(i, 0, 0), transform.rotation);
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}
}
