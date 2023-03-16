using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;

public class ObstacleGenerator : MonoBehaviour
{
    enum ObstacleType
    {
        obstacle, spike, doubleSpike, tripleSpike, trampolineSpike, trampoline, interactiveTrampoline
    }

    // PRUEBAS Graves Y Agudos
    //[SerializeField] private GameObject badPrefab;
    [SerializeField] private GameObject waterPrefab;
    //[SerializeField] private GameObject coinPrefab;

    [SerializeField] private GameObject features;
    [SerializeField] private Zone zones;

    // Prefabs
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject groundStartPrefab;
    [SerializeField] private GameObject[] obstacles;

    // Obstacle Array
    private readonly ObstacleType[] allObstacles = {ObstacleType.obstacle, ObstacleType.spike,
                                                    ObstacleType.doubleSpike, ObstacleType.tripleSpike,
                                                    ObstacleType.trampolineSpike};
    private readonly ObstacleType[] centerObstacles = {ObstacleType.obstacle, ObstacleType.spike,
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
        //List<float> agudosTiempo = features.GetComponent<ReadTxt>().getAgudosTiempo();
        //List<float> gravesTiempo = features.GetComponent<ReadTxt>().getGravesTiempo();
        //List<float> agudosValoresNorm = features.GetComponent<ReadTxt>().getAgudosValoresNorm();
        //List<float> gravesValoresNorm = features.GetComponent<ReadTxt>().getGravesValoresNorm();

        int iniH = zones.getBeatIniHigh();
        int iniL = zones.getBeatIniLow();
        int endH = zones.getBeatEndHigh();
        int endL = zones.getBeatEndLow();

        List<int> beatsZonesIndex = zones.getBeatZonesIndexes();


        //Debug.Log("Ini BEAT high: " + iniH);
        //Debug.Log("Fin BEAT high: " + endH);

        //Debug.Log("Ini BEAT low: " + iniL);
        //Debug.Log("Fin BEAT low: " + endL);

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getPlayerSpeed();
        GenerateLevel(width, height, beats, scopt, beatsZonesIndex);

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

    private void GenerateLevel(float width, float height, List<float> beats, List<float> scopt, List<int> beatsZonesIndex)
    {
        bool portal = false;
        int offsetI = 2;
        for (int i = offsetI; i < beats.Count() - 1; i++)
        {
            float x = beats[i] * multiplierX;
            int y = (int)(scopt[i] * multiplierY);
            if (i == offsetI)
            {
                Ground0_1(-1, y, x + 1, width, height);
                continue;
            }

            foreach (int b in beatsZonesIndex)
            {
                if (b == i)
                {
                    portal = true;
                    break;
                }
            }

            float prevX = beats[i - 1] * multiplierX;
            float nextX = beats[i + 1] * multiplierX;
            int prevY = (int)(scopt[i - 1] * multiplierY);
            int nextY = (int)(scopt[i + 1] * multiplierY);
            float distance = x - prevX;

            if (portal)
            {
                Instantiate(waterPrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
                Ground0_1(prevX, y, distance, width, height);
                portal = false;
                continue;
            }

            if (y-prevY >= 4)
            {
                InstantiateObstacle3Up(x, y, prevX, prevY, y-prevY);
                continue;
            }
            else if (y - prevY == 3)
            {
                InstantiateObstacle3Up(x, y, prevX, prevY, y-prevY);
                continue;
            }
            Ground0_1(prevX, y, distance, width, height);
            if (y - prevY == 0 && nextY - y == 0) InstantiateRandomObstacle(x, y);
            else if (nextY - y == 2) InstantiateObstacle2Up(x, y);
            else Instantiate(obstacles[(int)ObstacleType.obstacle], new Vector3(x, y, 0), transform.rotation, obstaclePool);
        }

    }

    private void Obstacle(float x, int y, float prevX, int prevY)
    {
        Instantiate(obstacles[(int)ObstacleType.obstacle], new Vector3((x + prevX) / 2f, (y + prevY) / 2f, 0), transform.rotation, obstaclePool);
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

    private void InstantiateObstacle2Up(float x, int y)
    {
        Instantiate(obstacles[(int)ObstacleType.spike], new Vector2(x, y), transform.rotation, obstaclePool);
    }

    private void InstantiateObstacle3Up(float x, int y, float prevX, int prevY, int highDiference)
    {
        int rnd = Random.Range(0, centerObstacles.Length);
        Destroy(obstaclePool.GetChild(obstaclePool.childCount - 1).gameObject);
        GameObject intTramp =  Instantiate(obstacles[(int)ObstacleType.interactiveTrampoline], new Vector2((x + prevX) / 2f, (y + prevY) / 2f), transform.rotation, obstaclePool);
        //intTramp.GetComponent<Trampoline>().setJumpForce(highDiference - 3);
        Instantiate(obstacles[(int)centerObstacles[rnd]], new Vector2(x, y), transform.rotation, obstaclePool);
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