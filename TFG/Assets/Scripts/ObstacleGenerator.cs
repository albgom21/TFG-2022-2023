using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    // PRUEBAS Graves Y Agudos
    [SerializeField] private GameObject badPrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject groundStartPrefab;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject features;
    [SerializeField] private Transform contenedorObs;
    [SerializeField] private Transform contenedorGround;
    [SerializeField] private int multiplierY;
    [SerializeField] private Sprite ini;
    [SerializeField] private Sprite mid;
    [SerializeField] private Sprite fin;
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

        for (int i = 0; i < beats.Count(); i++)
        {
            float x = beats[i] * multiplierX;
            int y = (int)(scopt[i] * 10);

            if (i > 0)
            {
                float prevX = beats[i - 1] * multiplierX;
                float distance = x - prevX;
                int prevY = (int)(scopt[i - 1] * 10);
                if (y - prevY == 3) Obstacle3(height, y, prevX, prevY, distance);
                else if (y - prevY == 2) Obstacle2(height, y, prevX, distance);
                else Obstacle(height, y, prevX, distance);
            }
            else if (i == 0)
            {
                for (int j = 0; j < 10; j++)
                    Instantiate(groundStartPrefab, new Vector3(x - width * j, y - height, 0), transform.rotation, contenedorGround);
            }

            Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, contenedorObs);
        }

        // PRUEBAS Graves Y Agudos
        for (int i = 0; i < gravesTiempo.Count(); i++)
        {
            float prevX = 0;
            float x = gravesTiempo[i] * multiplierX;
            if(i != 0)
                prevX = gravesTiempo[i - 1] * multiplierX;
            float distance = x - prevX;
            if (gravesValoresNorm[i] >= 0.8f)
                Instantiate(badPrefab, new Vector3(distance / 2 + prevX,5, 0), transform.rotation, contenedorObs);
        }
        for (int i = 0; i < agudosTiempo.Count(); i++)
        {
            float prevX = 0;
            float x = agudosTiempo[i] * multiplierX;
            if (i != 0)
                prevX = agudosTiempo[i - 1] * multiplierX;
            float distance = x - prevX;
            if (agudosValoresNorm[i] >= 0.8f)
                Instantiate(waterPrefab, new Vector3(distance / 2 + prevX, 4, 0), transform.rotation, contenedorObs);
        }

    }

    private void Obstacle(float height, int y, float prevX, float distance)
    {
        float obstacleWidth = obstaclePrefab.transform.localScale.x / 2;
        GameObject ground = Instantiate(groundPrefab, new Vector3(prevX + distance / 2 + obstacleWidth, y - height, 0), transform.rotation, contenedorGround);
        ground.transform.localScale = new Vector3(distance, ground.transform.localScale.y, ground.transform.localScale.z);
    }
    private void Obstacle2(float height, int y, float prevX, float distance)
    {
        GameObject ground = Instantiate(groundPrefab, new Vector3(distance / 2 + prevX + 1, y - height, 0), transform.rotation, contenedorGround);
        ground.transform.localScale = new Vector3(distance - 1, ground.transform.localScale.y, ground.transform.localScale.z);
    }
    private void Obstacle3(float height, int y, float prevX, float prevY, float distance)
    {
        Instantiate(obstaclePrefab, new Vector3(distance / 2 + prevX, (y + prevY) / 2, 0), transform.rotation, contenedorObs);
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