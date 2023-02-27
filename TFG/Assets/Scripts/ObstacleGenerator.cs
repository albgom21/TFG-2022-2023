using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject groundStartPrefab;
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

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getPlayerSpeed();

        for (int i = 0; i < beats.Count(); i++)
        {
            float x = beats[i] * multiplierX;
            int y = (int)(scopt[i] * 10 - 1);

            if (y > 0 && i > 0)
            {
                float prevX = beats[i - 1] * multiplierX;
                float distance = x - prevX;
                int prevY = (int)(scopt[i - 1] * 10 - 1);
                if (y - prevY == 3) Obstacle3(height, y, prevX, prevY, distance);
                else if(y -prevY ==2) Obstacle2(height, y, prevX, distance);
                else Obstacle(height, y, prevX, distance);
            }
            else if (i == 0)
            {
                for (int j = 0; j < 10; j++)
                {
                    //if (j == 0) groundPrefab.GetComponent<SpriteRenderer>().sprite = fin;
                    //else if (j == 9) groundPrefab.GetComponent<SpriteRenderer>().sprite = ini;
                    //else groundPrefab.GetComponent<SpriteRenderer>().sprite = mid;
                    Instantiate(groundStartPrefab, new Vector3(x - width * j, y - height, 0), transform.rotation, contenedorGround);
                }
            }
            Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, contenedorObs);
        }
        //float distance = (60.0f / BPM) * 10.0f; //Distancia entre bloques = 60 / BPM (segundos en 1 minuto) * 10 (el player recorre 10 unidades por segundo)
        //for (float i = distance; i < finalLevel; i += distance)
        //{
        //    Instantiate(obstaclePrefab, new Vector3(i, 0, 0), transform.rotation);
        //}
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