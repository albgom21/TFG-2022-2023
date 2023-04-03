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
    //[SerializeField] private GameObject coinPrefab;

    [SerializeField] private GameObject features;
    [SerializeField] private Zone zones;

    // Prefabs
    [SerializeField] private GameObject groundPrefab;
    private GameObject[] obstaclesStructures; //Estructuras NORMALES

        //Estructuras que contengan POWER UPS
    private GameObject[] gravityStructures;
    private GameObject[] slowMotionStructures;
    private GameObject[] lowResStructures;

    private int gravityStartIndex, gravityEndIndex; //Va a haber SIEMPRE solo dos power ups de Gravedad (uno que activa y otro desactiva)
    private List<int> slowMotionIndexes; //Lista de index donde se crearán los powerUps de slowMotion
    private List<int> lowResIndexes; //Lista de index donde se crearán los powerUps de lowRes

    [SerializeField] private GameObject endPrefab;
    [SerializeField] private GameObject groundToDestroy;

    // Pools
    [SerializeField] private Transform obstaclePool;
    [SerializeField] private Transform groundPool;

    // Multipliers
    private float multiplierX;

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

        obstaclesStructures     = Resources.LoadAll<GameObject>("Prefabs/Alvaro/Estructuras");
        gravityStructures       = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/Gravity");
        slowMotionStructures    = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/SlowMotion");
        lowResStructures    = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/LowRes");

        lastObstacle = null;
        //float width = transform.localScale.x;
        //float height = transform.localScale.y;
        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        zonesData = zones.getZonesData();

        //List<float> scopt = features.GetComponent<ReadTxt>().getScopt();

        //List<int> beatsZonesIndex = zones.getBeatZonesIndexes();

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getPlayerSpeed();
        //Debug.Log(minDistanceBetweenObstacles);

        GeneratePowerUpsIndexes(beats);

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
        float coordY = 0;
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

                //CREACIÓN DEL OBSTÁCULO
                thisObstacle = InstantiateRandomObstacle(getPosibleStructures(i), coordX, coordY, spaceBetweenBeats);

                //CREACIÓN DEL SUELO
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

    private ObstacleStructureData InstantiateRandomObstacle(GameObject[] posibleStructures, float x, float y, float spaceBetweenBeats)
    {
        bool correctObstacle = false;
        GameObject obstacle;
        ObstacleStructureData obstacleStructure = null;

        int intentos = 0;
        while (!correctObstacle && intentos < 40)
        {
            int rnd = Random.Range(0, posibleStructures.Length);

            ChangeGroundColor(posibleStructures,rnd, lowColorChange, highColorChange);

            obstacle = Instantiate(posibleStructures[rnd], new Vector3(x, y, 0), transform.rotation, obstaclePool);

            obstacleStructure = obstacle.GetComponent<ObstacleStructureData>();

            if (lastObstacle == null) correctObstacle = true; //Esto solo se da en el primer obstáculo, aún no hay anterior
            else correctObstacle = obstacleEnabled(obstacleStructure) && (lastObstacle.getPostX() + obstacleStructure.getPrevX() < spaceBetweenBeats); //Comprueba si es correcto

            if (!correctObstacle)
            {
                Destroy(obstacle); //Si no lo es, lo destruye, volverá al while y creará otro.
                obstacleStructure = null;
            }

            intentos++;
        }

        if (obstacleStructure == null)
        { //Si no se ha encontrado en todos los intentos ninguno
            //Pongo A MANO el número 0 porque es el más "fino", cabe seguro (es un obstáculo vacío)
            ChangeGroundColor(posibleStructures, 0, lowColorChange, highColorChange);
            obstacle = Instantiate(posibleStructures[0], new Vector3(x, y, 0), transform.rotation, obstaclePool);
            obstacleStructure = obstacle.GetComponent<ObstacleStructureData>();

            //No hay ningún problema aparente en que llegue hasta aquí, pero por saber pongo el Debug.Log 
            //(Especialmente no quiero que llegue aquí si debería de haber creado un powerUp)
            //Debug.Log("Aviso de generación de obstáculo no encontrada");
        }

        return obstacleStructure;
    }

    // Cambia el color del suelo a todos los "Ground" de un obstaculo struct según la zona
    private void ChangeGroundColor(GameObject[] structures, int index, bool lowColorChange, bool highColorChange)
    {
        // Recorrer todos los hijos del GameObject padre
        for (int i = 0; i < structures[index].transform.childCount; i++)
        {
            // Obtener el GameObject hijo actual
            GameObject hijo = structures[index].transform.GetChild(i).gameObject;

            // Comprobar si el GameObject hijo tiene el tag "Ground"
            if (hijo.CompareTag("Ground"))
            {
                if (lowColorChange)
                    hijo.GetComponent<SpriteRenderer>().color = lowColor;
                else if (highColorChange)
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

    //Devuelve si el obstáculo elegido está activado para usarlo y si es posible por su dificultad
    bool obstacleEnabled(ObstacleStructureData obstacleStructure)
    {
        int obstacleDif = obstacleStructure.getDifficulty();

        //Si la dificultad del nivel es -1, me da igual la dificultad del obstáculo
        //Si la dificultad del obstáculo es -1, me da igual la dificultad del nivel
        if (difficulty == -1 || obstacleDif == -1) return obstacleStructure.getObstacleEnabled(); 

        //Se elegirán obstáculos cuya dificultad sea igual o con 1 de diferencia respecto a la dificultad del script
        //Ejemplo: si la dificultad marcada en script es 3, se elegirá, obstáculos de dificultad 2, 3 y 4
        return obstacleStructure.getObstacleEnabled() && difficulty >= obstacleDif - 1 && difficulty <= obstacleDif + 1;
    }

    //Devuelve el array de estructuras que se va a utilizar para el siguiente beat
    private GameObject[] getPosibleStructures(int index)
    {
        //Estructuras de powerUp de gravedad

        if (index == gravityStartIndex || index == gravityEndIndex) return gravityStructures;
        //Estructuras de powerUp de slowMotion
        if (slowMotionIndexes.Contains(index)) return slowMotionStructures;

        //Estructuras de powerUp de LowRes
        if (lowResIndexes.Contains(index)) return lowResStructures;

        //Si no es ninguna de las anteriores, es cualquier obstáculo "normal"
        return obstaclesStructures; 
    }

    //Genera los index de los beats en los que va a haber powerUps
    private void GeneratePowerUpsIndexes(List<float> beats)
    {
        //GRAVITY POWER UP
        int gravityZoneLength = Random.Range(10, 30); //La zona con gravedad cambiada serán entre 10 y 30 beats
        int margin = 10; //Margen de la zona de gravedad. Es decir, los primeros "margin" beats no serán zona ni los últimos tampoco

        gravityStartIndex = Random.Range(margin, beats.Count - margin - gravityZoneLength);
        gravityEndIndex = gravityStartIndex + gravityZoneLength;
        Debug.Log("El comienzo de la zona Gravedad está creado en el beat " + gravityStartIndex);
        Debug.Log("El final de la zona Gravedad está creado en el beat " + gravityEndIndex);


        //SLOW MOTION POWER UP
        slowMotionIndexes = new List<int>();

        int numSlowMotions = Random.Range(1, 4); //Va a haber entre 1 y 3 (el 4 es excluído) slowMotions
        int marginBetweenSMPowerUps = 15; //Mínimo tiene que haber 15 beats entre un SlowMotion y otro

        for (int i = 0; i < numSlowMotions; ++i) //Para cada slowMotion que se vaya a crear
        {
            int randomStart;
            if (i == 0) randomStart = margin;
            else randomStart = slowMotionIndexes[i - 1] + marginBetweenSMPowerUps;

            int randomEnd;
            if (i == numSlowMotions - 1) randomEnd = margin;
            else
            {
                int powerUpsLeft = numSlowMotions - 1 - i; //Numero de powerUps que aún quedan por poner para calcular los márgenes
                randomEnd = beats.Count - margin - (marginBetweenSMPowerUps * powerUpsLeft);
            }

            int newSlowMotionIndex = Random.Range(randomStart, randomEnd);
            slowMotionIndexes.Add(newSlowMotionIndex);

            Debug.Log("Power Up de SlowMotion creado en el beat " + newSlowMotionIndex);
        }


        //LOW RES POWER UP
        lowResIndexes = new List<int>();

        int numLowRes = Random.Range(1, 3); //Va a haber entre 1 y 2 (el 3 es excluído) LowRes
        int marginBetweenBQPowerUps = 30; //Mínimo tiene que haber 30 beats entre un SlowMotion y otro

        for (int i = 0; i < numLowRes; ++i) //Para cada LowRes que se vaya a crear
        {
            int randomStart;
            if (i == 0) randomStart = margin;
            else randomStart = lowResIndexes[i - 1] + marginBetweenBQPowerUps;

            int randomEnd;
            if (i == numLowRes - 1) randomEnd = margin;
            else
            {
                int powerUpsLeft = numLowRes - 1 - i; //Numero de powerUps que aún quedan por poner para calcular los márgenes
                randomEnd = beats.Count - margin - (marginBetweenBQPowerUps * powerUpsLeft);
            }

            int newLowResIndex = Random.Range(randomStart, randomEnd);
            lowResIndexes.Add(newLowResIndex);

            Debug.Log("Power Up de LowRes creado en el beat " + newLowResIndex);
        }
    }
}