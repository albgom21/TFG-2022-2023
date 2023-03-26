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
    private GameObject[] obstaclesStructures;
    [SerializeField] private GameObject endPrefab;
    [SerializeField] private GameObject groundToDestroy;

    // Pools
    [SerializeField] private Transform obstaclePool;
    [SerializeField] private Transform groundPool;

    // Multipliers
    private float multiplierX;
    private float minDistanceBetweenObstacles;

    //public int BPM;
    //public float finalLevel;
    private ObstacleStructureData lastObstacle;

    void Start()
    {
        obstaclesStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/Estructuras");
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

        Instantiate(endPrefab, new Vector3(50000, 0, 0), transform.rotation, obstaclePool);

        Destroy(groundToDestroy);
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
        ObstacleStructureData thisObstacle = null;


        for (int i = offsetI; i < beats.Count() - 1; i++)
        {
            coordX = beats[i] * multiplierX; //coordenada X del obstáculo.
            
            //Espacio entre el CENTRO del anterior obstáculo y este
            float spaceBetweenBeats = coordX - prevX; 

            if (lastObstacle == null || spaceBetweenBeats > lastObstacle.getPostX())
            {
                thisObstacle = InstantiateRandomObstacle(coordX, coordY, spaceBetweenBeats);

                //El suelo tendrá que ir desde el FINAL (no el centro) del anterior obstáculo hasta el PRINCIPIO de este
                float floorStart, floorEnd;

                if (lastObstacle == null) floorStart = 0.0f; //Solo ocurre con el primer obstáculo, el cual no tiene anterior
                else floorStart = prevX + lastObstacle.getPostX();

                floorEnd = coordX - thisObstacle.getPrevX();

                GenerateFloor(floorStart, floorEnd, coordY);
            }
            //if (portal)
            //{
            //    Instantiate(waterPrefab, new Vector3(x, y, 0), transform.rotation, obstaclePool);
            //    Ground0_1(prevX, y, distance, width, height);
            //    portal = false;
            //    continue;
            //}


            //Preparando la siguiente iteración
            if (thisObstacle != null)
            {
                prevX = coordX;
                coordY += thisObstacle.getUnlevel();
                lastObstacle = thisObstacle;
                thisObstacle = null;
            }
            
        }
    }

    private ObstacleStructureData InstantiateRandomObstacle(float x, float y, float spaceBetweenBeats)
    {
        bool correctObstacle = false;
        GameObject obstacle;
        ObstacleStructureData obstacleStructure = null;

        int intentos = 0;
        while (!correctObstacle && intentos < 25)
        {
            int rnd = Random.Range(0, obstaclesStructures.Length);
            obstacle = Instantiate(obstaclesStructures[rnd], new Vector3(x, y, 0), transform.rotation, obstaclePool);

            obstacleStructure = obstacle.GetComponent<ObstacleStructureData>();

            if (lastObstacle == null) correctObstacle = true; //Esto solo se da en el primer obstáculo, aún no hay anterior
            else correctObstacle = (obstacleStructure.getObstacleEnabled()) && (lastObstacle.getPostX() + obstacleStructure.getPrevX() < spaceBetweenBeats); //Comprueba si es correcto


            if (!correctObstacle)
            {
                Destroy(obstacle); //Si no lo es, lo destruye, volverá al while y creará otro.
                obstacleStructure = null;
            }

            intentos++;
        }

        if (obstacleStructure == null) //Si no se ha encontrado en todos los intentos ninguno
        {
            //Pongo A MANO el número 9 (Struct9) porque es el más "fino", cabe seguro
            obstacle = Instantiate(obstaclesStructures[9], new Vector3(x, y, 0), transform.rotation, obstaclePool);
            obstacleStructure = obstacle.GetComponent<ObstacleStructureData>();
        }

        return obstacleStructure;
    }

    private void GenerateFloor(float floorStart, float floorEnd, float y)
    {
        float width = floorEnd - floorStart;
        GameObject ground = Instantiate(groundPrefab, new Vector3(floorStart + (width / 2.0f), y, 0), transform.rotation, groundPool);
        ground.transform.localScale = new Vector3(width, ground.transform.localScale.y, ground.transform.localScale.z);
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