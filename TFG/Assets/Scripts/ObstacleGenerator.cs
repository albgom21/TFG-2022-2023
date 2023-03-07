using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Analytics;

public class ObstacleGenerator : MonoBehaviour
{
    // PRUEBAS Graves Y Agudos
    [SerializeField] private GameObject badPrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject coinPrefab;


    [SerializeField] private GameObject features;

    // Prefabs
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject groundStartPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject spikePrefab;
    //[SerializeField] private GameObject doubleSpikePrefab;
    //[SerializeField] private GameObject tripleSpikePrefab;

    // Pools
    [SerializeField] private Transform obstaclePool;
    [SerializeField] private Transform groundPool;

    // Multipliers
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

        // PRUEBAS Graves Y Agudos
        List<float> agudosTiempo = features.GetComponent<ReadTxt>().getAgudosTiempo();
        List<float> gravesTiempo = features.GetComponent<ReadTxt>().getGravesTiempo();
        List<float> agudosValoresNorm = features.GetComponent<ReadTxt>().getAgudosValoresNorm();
        List<float> gravesValoresNorm = features.GetComponent<ReadTxt>().getGravesValoresNorm();

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getPlayerSpeed();

        for (int i = 0; i < beats.Count() - 1; i++)
        {
            float x = beats[i] * multiplierX;
            int y = (int)(scopt[i] * multiplierY);
            if (i == 0)
            {
                for (int j = 0; j < 10; j++)
                    Instantiate(groundStartPrefab, new Vector3(x - width * j, y - height, 0), transform.rotation, groundPool);
                continue;
            }
            float prevX = beats[i - 1] * multiplierX;
            int prevY = (int)(scopt[i - 1] * multiplierY);
            int nextY = (int)(scopt[i + 1] * multiplierY);
            float distance = x - prevX;
            //if (y - prevY == 3) Obstacle3(height, x, y, prevX, prevY, distance);
            //else if (nextY - y == 2) Obstacle2(height, x, y, prevX, distance);
            if (y - prevY == 0 || y - prevY == 1)
            {
                Ground0_1(height, y, prevX, distance);
                if (nextY - y == 1) Instantiate(spikePrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
                else Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
            }
            else if (y - prevY == 2)
            {
                Instantiate(obstaclePrefab, new Vector3((x + prevX) / 2, y - 1, 0), transform.rotation, obstaclePool);
                Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
            }
            else if (y - prevY == -1)
            {
                Ground0_1(height, y, prevX, distance);
                Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
            }
            else
            {
                Ground0_1(height, y, prevX, distance);
                Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
            }
        }

        #region pruebas GyA
        // PRUEBAS Graves Y Agudos
        //for (int i = 0; i < gravesTiempo.Count(); i++)
        //{
        //    float prevX = 0;
        //    float x = gravesTiempo[i] * multiplierX;
        //    if (i != 0)
        //        prevX = gravesTiempo[i - 1] * multiplierX;
        //    float distance = x - prevX;
        //    if (gravesValoresNorm[i] >= 0.8f)
        //        Instantiate(badPrefab, new Vector3(distance / 2 + prevX, 5, 0), transform.rotation, obstaclePool);
        //}
        //for (int i = 0; i < agudosTiempo.Count(); i++)
        //{
        //    float prevX = 0;
        //    float x = agudosTiempo[i] * multiplierX;
        //    if (i != 0)
        //        prevX = agudosTiempo[i - 1] * multiplierX;
        //    float distance = x - prevX;
        //    if (agudosValoresNorm[i] >= 0.8f)
        //        Instantiate(waterPrefab, new Vector3(distance / 2 + prevX, 4, 0), transform.rotation, obstaclePool);
        //}
        #endregion
    }


    private void Obstacle(float height, float x, int y, float prevX, float distance)
    {
        float obstacleWidth = obstaclePrefab.transform.localScale.x / 2;
        GameObject ground = Instantiate(groundPrefab, new Vector3(prevX + distance / 2 + obstacleWidth, y - height, 0), transform.rotation, groundPool);
        ground.transform.localScale = new Vector3(distance, ground.transform.localScale.y, ground.transform.localScale.z);
        Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
    }

    private void Ground0_1(float height, int y, float prevX, float distance)
    {
        float obstacleWidth = obstaclePrefab.transform.localScale.x / 2;
        GameObject ground = Instantiate(groundPrefab, new Vector3(prevX + distance / 2 + obstacleWidth, y - height, 0), transform.rotation, groundPool);
        ground.transform.localScale = new Vector3(distance, ground.transform.localScale.y, ground.transform.localScale.z);
    }

    private void Obstacle2(float height, float x, int y, float prevX, float distance)
    {
        GameObject ground = Instantiate(groundPrefab, new Vector3(distance / 2 + prevX + 1, y - height, 0), transform.rotation, groundPool);
        ground.transform.localScale = new Vector3(distance - 1, ground.transform.localScale.y, ground.transform.localScale.z);
        Instantiate(spikePrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
    }
    private void Obstacle3(float height, float x, int y, float prevX, float prevY, float distance)
    {
        //Instantiate(obstaclePrefab, new Vector3(distance / 2 + prevX, (y + prevY) / 2, 0), transform.rotation, obstaclePool);
        //Transform aux = groundPool.GetChild(groundPool.transform.childCount - 1);
        //aux.localScale = new Vector3(2, 1, 1);
        //aux.position = new Vector3(prevX + 1 + (aux.localScale.x / 2), y, 0);
    }

    public float getMultiplierX()
    {
        return multiplierX;
    }

    public int getMultiplierY()
    {
        return multiplierY;
    }

    public GameObject getFeatures()
    {
        return features;
    }
}