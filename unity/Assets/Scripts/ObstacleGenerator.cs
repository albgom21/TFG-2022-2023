using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    enum ObstacleType
    {
        obstacle, spike, doubleSpike, tripleSpike, trampolineSpike
    }

    // PRUEBAS Graves Y Agudos
    [SerializeField] private GameObject badPrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private GameObject features;

    // Prefabs
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject groundStartPrefab;
    [SerializeField] private GameObject[] obstacles;

    // Obstacle Array
    private readonly ObstacleType[] allObstacles = {ObstacleType.obstacle, ObstacleType.spike,
                                                    ObstacleType.doubleSpike, ObstacleType.tripleSpike,
                                                    ObstacleType.trampolineSpike};

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
                Instantiate(obstacles[(int)ObstacleType.obstacle], new Vector3(x, y, 0), transform.rotation, obstaclePool);
                continue;
            }
            float prevX = beats[i - 1] * multiplierX;
            float nextX = beats[i + 1] * multiplierX;
            int prevY = (int)(scopt[i - 1] * multiplierY);
            int nextY = (int)(scopt[i + 1] * multiplierY);
            float distance = x - prevX;


            Ground0_1(prevX, y, distance, width, height);

            if (nextY - y == 0)
            {
                InstantiateRandomObstacle(x, y);
            }
            else Instantiate(obstacles[(int)ObstacleType.obstacle], new Vector3(x, y, 0), transform.rotation, obstaclePool);

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
        //float obstacleWidth = obstaclePrefab.transform.localScale.x / 2;
        //GameObject ground = Instantiate(groundPrefab, new Vector3(prevX + distance / 2 + obstacleWidth, y - height, 0), transform.rotation, groundPool);
        //ground.transform.localScale = new Vector3(distance, ground.transform.localScale.y, ground.transform.localScale.z);
        //Instantiate(obstaclePrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
    }

    private void Ground0_1(float prevX, int y, float distance, float width, float height)
    {
        GameObject ground = Instantiate(groundPrefab, new Vector3(prevX + distance / 2 + width / 2, y - height, 0), transform.rotation, groundPool);
        ground.transform.localScale = new Vector3(distance, ground.transform.localScale.y, ground.transform.localScale.z);
    }

    private void InstantiateRandomObstacle(float x, float y)
    {
        int rnd = Random.Range(0, allObstacles.Length);
        Instantiate(obstacles[(int)allObstacles[rnd]], new Vector2(x, y), transform.rotation, obstaclePool);
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