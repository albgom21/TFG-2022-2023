using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;

public class AlObstacleGenerator : MonoBehaviour
{

    // PRUEBAS Graves Y Agudos
    //[SerializeField] private GameObject badPrefab;
    [SerializeField] private GameObject waterPrefab;
    //[SerializeField] private GameObject coinPrefab;

    [SerializeField] private GameObject features;
    //[SerializeField] private Zone zones;

    // Prefabs
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject groundStartPrefab;
    [SerializeField] private GameObject[] obstaclesStructures;

    // Pools
    [SerializeField] private Transform obstaclePool;
    [SerializeField] private Transform groundPool;

    // Multipliers
    private float multiplierX;
    private float minDistanceBetweenObstacles;

    //public int BPM;
    //public float finalLevel;
    private ObstacleStructure lastObstacle;

    void Start()
    {
        //obstaclesStructures = Resources.LoadAll<GameObject>("../Prefabs/Alvaro/Estructuras");
        lastObstacle = null;
        //float width = transform.localScale.x;
        //float height = transform.localScale.y;
        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        //List<float> scopt = features.GetComponent<ReadTxt>().getScopt();

        //int iniH = zones.getBeatIniHigh();
        //int iniL = zones.getBeatIniLow();
        //int endH = zones.getBeatEndHigh();
        //int endL = zones.getBeatEndLow();

        //Debug.Log("Ini BEAT high: " + iniH);
        //Debug.Log("Fin BEAT high: " + endH);

        //Debug.Log("Ini BEAT low: " + iniL);
        //Debug.Log("Fin BEAT low: " + endL);

        //List<int> beatsZonesIndex = zones.getBeatZonesIndexes();

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getPlayerSpeed();
        minDistanceBetweenObstacles = multiplierX / 2.0f;
        Debug.Log(minDistanceBetweenObstacles);

        GenerateObstacles(beats/*, beatsZonesIndex*/);
    }

    private void GenerateObstacles(List<float> beats/*, List<int> beatsZonesIndex*/)
    {
        //bool portal = false;
        int offsetI = 2;
        //bool jumpObstacle = false;
        //Coordenadas del obstáculo
        float coordX;
        float coordY = transform.localScale.y;
        float prevX = 0.0f;
  
        for (int i = offsetI; i < beats.Count() - 1; i++)
        {
            coordX = beats[i] * multiplierX; //coordenada X del obstáculo.

            
            //Espacio entre el CENTRO del anterior obstáculo y este
            float spaceBetweenBeats = coordX - prevX; 
            ObstacleStructure thisObstacle = InstantiateRandomObstacle(coordX, coordY, spaceBetweenBeats);

            float floorStart, floorEnd;

            if (lastObstacle == null) floorStart = 0.0f;
            else floorStart = prevX + lastObstacle.getPostX();

            floorEnd = coordX - thisObstacle.getPrevX();

            GenerateFloorBetweenObstacles(floorStart, floorEnd, coordY);

            //if (portal)
            //{
            //    Instantiate(waterPrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
            //    Ground0_1(prevX, y, distance, width, height);
            //    portal = false;
            //    continue;
            //}


            //Preparando la siguiente iteración
            prevX = coordX;
            if (thisObstacle != null) coordY += thisObstacle.getUnlevel();
            lastObstacle = thisObstacle;
        }
    }

    private ObstacleStructure InstantiateRandomObstacle(float x, float y, float spaceBetweenBeats)
    {
        bool correctObstacle = false;
        GameObject obstacle;
        ObstacleStructure obstacleStructure = null;

        int intentos = 0;
        while (!correctObstacle && intentos < 20)
        {
            int rnd = Random.Range(0, obstaclesStructures.Length);
            obstacle = Instantiate(obstaclesStructures[rnd], new Vector3(x, y, 0), transform.rotation, obstaclePool);

            obstacleStructure = obstacle.GetComponent<ObstacleStructure>();

            if (lastObstacle == null) correctObstacle = true; //Esto solo se da en el primer obstáculo, aún no hay anterior
            else correctObstacle = (lastObstacle.getPostX() + obstacleStructure.getPrevX() < spaceBetweenBeats); //Comprueba si es correcto


            if (!correctObstacle) Destroy(obstacle); //Si no lo es, lo destruye, volverá al while y creará otro.
            intentos++;
        }

        if (obstacleStructure == null) //Si no se ha encontrado en todos los intentos ninguno
        {
            //Pongo A MANO el número 9 porque es el más "fino", cabe seguro
            obstacle = Instantiate(obstaclesStructures[9], new Vector3(x, y, 0), transform.rotation, obstaclePool);
            obstacleStructure = obstacle.GetComponent<ObstacleStructure>();
        }

        return obstacleStructure;
    }

    private void GenerateFloorBetweenObstacles(float prevObstX, float obstX, float y)
    {
        float distanceX = obstX - prevObstX;
        GameObject ground = Instantiate(groundPrefab, new Vector3(prevObstX + (distanceX / 2.0f), y, 0), transform.rotation, groundPool);
        ground.transform.localScale = new Vector3(distanceX, ground.transform.localScale.y, ground.transform.localScale.z);
    }

    public float getMultiplierX()
    {
        return multiplierX;
    }

    public GameObject getFeatures()
    {
        return features;
    }
}