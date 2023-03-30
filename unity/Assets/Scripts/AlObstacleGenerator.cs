using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using ZoneCode;

public class AlObstacleGenerator : MonoBehaviour
{

    // PRUEBAS Graves Y Agudos
    //[SerializeField] private GameObject badPrefab;
    [SerializeField] private GameObject waterPrefab;
    //[SerializeField] private GameObject coinPrefab;

    [SerializeField] private GameObject features;
    [SerializeField] private Zone zones;

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

    private ObstacleStructureData lastObstacle;

    // Dificultad
    [SerializeField] private int difficulty;

    // Zones
    List<ZoneData> zonesData;
    Color lowColor, highColor;
    bool lowColorChange = false;
    bool highColorChange = false;
    

    void Start()
    {
        lowColor = new Color(0.2f, 0.4f, 0.6f, 1.0f);  // DARK BLUE
        highColor = new Color(1.0f, 1.0f, 0.0f, 1.0f); // YELLOW

        obstaclesStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/Estructuras");
        lastObstacle = null;
        //float width = transform.localScale.x;
        //float height = transform.localScale.y;
        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        zonesData = zones.getZonesData();

        //List<float> scopt = features.GetComponent<ReadTxt>().getScopt();

        //List<int> beatsZonesIndex = zones.getBeatZonesIndexes();

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getPlayerSpeed();
        minDistanceBetweenObstacles = multiplierX / 2.0f;
        //Debug.Log(minDistanceBetweenObstacles);

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
            groundPrefab.GetComponent<SpriteRenderer>().color = Color.white;
            foreach (ZoneData z in zonesData)
            {
                /*Debug.Log("N: " + cont);
                Debug.Log("TYPE: " + z.getType());
                Debug.Log("B INI: " + z.getBeatIni());
                Debug.Log("B END: " + z.getBeatEnd());
                Debug.Log("T INI: " + z.getTimeIniZone());
                Debug.Log("T END: " + z.getTimeEndZone());*/

                if (i >= z.getBeatIni() && i <= z.getBeatEnd())
                {
                    if (z.getType() == ZoneType.LOW)
                    {
                        groundPrefab.GetComponent<SpriteRenderer>().color = lowColor;
                        lowColorChange = true;
                    }
                    else if (z.getType() == ZoneType.HIGH)
                    {
                        groundPrefab.GetComponent<SpriteRenderer>().color = highColor;
                        highColorChange = true;
                    }

                    break;
                }
                lowColorChange = false;
                highColorChange = false;

            }

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

                    ChangeGroundColor(rnd, lowColorChange, highColorChange);
            
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

        if (obstacleStructure == null){ //Si no se ha encontrado en todos los intentos ninguno
            ChangeGroundColor(9,lowColorChange, highColorChange);
            //Pongo A MANO el número 9 (Struct9) porque es el más "fino", cabe seguro
            obstacle = Instantiate(obstaclesStructures[9], new Vector3(x, y, 0), transform.rotation, obstaclePool);
            obstacleStructure = obstacle.GetComponent<ObstacleStructureData>();
        }

        return obstacleStructure;
    }

    // Cambia el color del suelo a todos los "Ground" de un obstaculo struct según la zona
    private void ChangeGroundColor(int index, bool lowColorChange, bool highColorChange)
    {
        // Recorrer todos los hijos del GameObject padre
        for (int i = 0; i < obstaclesStructures[index].transform.childCount; i++)
        {
            // Obtener el GameObject hijo actual
            GameObject hijo = obstaclesStructures[index].transform.GetChild(i).gameObject;

            // Comprobar si el GameObject hijo tiene el tag "Ground"
            if (hijo.CompareTag("Ground"))
            {
                if (lowColorChange)
                    hijo.GetComponent<SpriteRenderer>().color = lowColor;
                else if(highColorChange)
                    hijo.GetComponent<SpriteRenderer>().color = highColor;
                else
                    hijo.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
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

    bool obstacleEnabled(ObstacleStructureData obstacleStructure)
    {
        if (difficulty == -1) return obstacleStructure.getObstacleEnabled();

        //else
        int obstacleDif = obstacleStructure.getDifficulty();
        //Se elegirán obstáculos cuya dificultad sea igual o con 1 de diferencia respecto a la dificultad del script
        //Ejemplo: si la dificultad marcada en script es 3, se elegirá, obstáculos de dificultad 2, 3 y 4
        return obstacleStructure.getObstacleEnabled() && difficulty >= obstacleDif - 1 && difficulty <= obstacleDif + 1; 
    }
}