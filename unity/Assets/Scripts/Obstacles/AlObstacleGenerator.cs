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

    //Estructuras de comienzos/finales de zonas
    private GameObject[] lowZoneStartStructures;
    private GameObject[] lowZoneEndStructures;

    //Estructuras que contengan POWER UPS
    private GameObject[] gravityStructures;
    private GameObject[] slowMotionStructures;
    private GameObject[] lowResStructures;

    private int gravityStartIndex, gravityEndIndex; //Va a haber SIEMPRE solo dos power ups de Gravedad (uno que activa y otro desactiva)
    private List<int> slowMotionIndexes; //Lista de index donde se crear�n los powerUps de slowMotion
    private List<int> lowResIndexes; //Lista de index donde se crear�n los powerUps de lowRes

    private List<int> importantIndexes; //Lista de index con TODOS los structs especiales (powerUps, cambios de zona etc)

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

    private int lowZoneStartIndex, lowZoneEndIndex;

    void Start()
    {
        lowColor = new Color(0.2f, 0.4f, 0.6f, 1.0f);  // DARK BLUE
        highColor = new Color(1.0f, 1.0f, 0.0f, 1.0f); // YELLOW

        obstaclesStructures     = Resources.LoadAll<GameObject>("Prefabs/Alvaro/Estructuras");
        lowZoneStartStructures  = Resources.LoadAll<GameObject>("Prefabs/Alvaro/ChangeZone/LowZoneStart");
        lowZoneEndStructures    = Resources.LoadAll<GameObject>("Prefabs/Alvaro/ChangeZone/LowZoneEnd");
        gravityStructures       = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/Gravity");
        slowMotionStructures    = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/SlowMotion");
        lowResStructures        = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/LowRes");

        lastObstacle = null;

        List<float> beats = features.GetComponent<ReadTxt>().getBeatsInTime();
        zonesData = zones.getZonesData();

        importantIndexes = new List<int>();

        //List<float> scopt = features.GetComponent<ReadTxt>().getScopt();

        //List<int> beatsZonesIndex = zones.getBeatZonesIndexes();

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getPlayerSpeed();
        //Debug.Log(minDistanceBetweenObstacles);

        InitZonesIndexes();

        GeneratePowerUpsIndexes(beats);

        GenerateObstacles(beats/*, beatsZonesIndex*/);

  
        Destroy(groundToDestroy);
    }

    private void GenerateObstacles(List<float> beats/*, List<int> beatsZonesIndex*/)
    {
        //bool portal = false;
        int offsetI = 2;
        //Coordenadas del obst�culo y del previo
        float coordX, coordY, prevX;    coordX = coordY = prevX = 0.0f;
        ObstacleStructureData thisObstacle = null;

        bool zoneIsChanging;
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

            coordX = beats[i] * multiplierX; //coordenada X del obst�culo.

            //Espacio entre el CENTRO del anterior obst�culo y este
            float spaceBetweenBeats = coordX - prevX;

            if (lastObstacle == null || spaceBetweenBeats > lastObstacle.getPostX())
            {
                //CREACI�N DEL OBST�CULO

                //Si este index es justo el anterior a donde va a haber uno importante, quiero instanciar uno vac�o (Default).
                if (importantIndexes.Contains(i + 1)) thisObstacle = InstantiateDefaultObstacle(getPosibleStructures(i), coordX, coordY);
                    

                //Si no, uno aleatorio
                else
                    thisObstacle = InstantiateRandomObstacle(getPosibleStructures(i), coordX, coordY, spaceBetweenBeats);

                //CREACI�N DEL SUELO
                //El suelo tendr� que ir desde el FINAL (no el centro) del anterior obst�culo hasta el PRINCIPIO de este
                float floorStart, floorEnd;

                if (lastObstacle == null) floorStart = 0.0f; //Solo ocurre con el primer obst�culo, el cual no tiene anterior
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


            //Preparando la siguiente iteraci�n
            if (thisObstacle != null)
            {
                prevX = coordX;
                coordY += thisObstacle.getUnlevel();
                lastObstacle = thisObstacle;
                thisObstacle = null; //Si es el �ltimo, quiero que se guarde para la creaci�n del END
            }
        }

        //END
        coordX = coordX + lastObstacle.getPostX() + (endPrefab.transform.localScale.x / 2);
        //Coord X estaba en el centro del beat anterior, ahora est� tras el resto del obst�culo anterior y movido
        //a la mitad del prefab de end

        Instantiate(endPrefab, new Vector3(coordX , coordY, 0), transform.rotation, obstaclePool);
    }

    private ObstacleStructureData InstantiateRandomObstacle(GameObject[] posibleStructures, float x, float y, float spaceBetweenBeats)
    {
        bool correctObstacle = false;
        ObstacleStructureData obstacleStructure = null;

        int intentos = 0;
        while (!correctObstacle && intentos < 50)
        {
            int rnd = Random.Range(0, posibleStructures.Length);

            ChangeGroundColor(posibleStructures,rnd, lowColorChange, highColorChange);

            GameObject obstacle = Instantiate(posibleStructures[rnd], new Vector3(x, y, 0), transform.rotation, obstaclePool);

            obstacleStructure = obstacle.GetComponent<ObstacleStructureData>();

            if (lastObstacle == null) correctObstacle = true; //Esto solo se da en el primer obst�culo, a�n no hay anterior
            else correctObstacle = ObstacleEnabled(obstacleStructure) && (lastObstacle.getPostX() + obstacleStructure.getPrevX() < spaceBetweenBeats); //Comprueba si es correcto

            if (!correctObstacle)
            {
                Destroy(obstacle); //Si no lo es, lo destruye, volver� al while y crear� otro.
                obstacleStructure = null;
            }

            intentos++;
        }

        //Si ha llegado hasta aqu� siendo null, significa que tras todos los intentos no ha encontrado ninguno que quepe.
        //Pongo A MANO el n�mero 0 porque es el m�s "fino", cabe seguro (es un obst�culo vac�o)
        if (obstacleStructure == null) obstacleStructure = InstantiateDefaultObstacle(posibleStructures, x, y);

        return obstacleStructure;
    }

    //Instancia dentro de los posiblesStructures el por defecto (0) en las coordenadas x,y
    private ObstacleStructureData InstantiateDefaultObstacle(GameObject[] posibleStructures, float x, float y)
    {
        ChangeGroundColor(posibleStructures, 0, lowColorChange, highColorChange);
        GameObject defaultObstacle = Instantiate(posibleStructures[0], new Vector3(x, y, 0), transform.rotation, obstaclePool);
        return defaultObstacle.GetComponent<ObstacleStructureData>();
    }

    // Cambia el color del suelo a todos los "Ground" de un obstaculo struct seg�n la zona
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

    //Genera un suelo en la altura y entre las coordenadasX floorStart y floorEnd
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

    //Devuelve si el obst�culo elegido est� activado para usarlo y si es posible por su dificultad
    private bool ObstacleEnabled(ObstacleStructureData obstacleStructure)
    {
        int obstacleDif = obstacleStructure.getDifficulty();

        //Si la dificultad del nivel es -1, me da igual la dificultad del obst�culo
        //Si la dificultad del obst�culo es -1, me da igual la dificultad del nivel
        if (difficulty == -1 || obstacleDif == -1) return obstacleStructure.getObstacleEnabled(); 

        //Se elegir�n obst�culos cuya dificultad sea igual o con 1 de diferencia respecto a la dificultad del script
        //Ejemplo: si la dificultad marcada en script es 3, se elegir�, obst�culos de dificultad 2, 3 y 4
        return obstacleStructure.getObstacleEnabled() && difficulty >= obstacleDif - 1 && difficulty <= obstacleDif + 1;
    }

    //Devuelve el array de estructuras que se va a utilizar para el siguiente beat
    private GameObject[] getPosibleStructures(int index)
    {
        //Estructuras de comienzo de zonaLow (agua)
        if (index == lowZoneStartIndex) return lowZoneStartStructures;

        //Estructuras de final de zonaLow (agua)
        if (index == lowZoneEndIndex) return lowZoneEndStructures;

        //Estructuras de powerUp de gravedad
        if (index == gravityStartIndex || index == gravityEndIndex) return gravityStructures;

        //Estructuras de powerUp de slowMotion
        if (slowMotionIndexes.Contains(index)) return slowMotionStructures;

        //Estructuras de powerUp de LowRes
        if (lowResIndexes.Contains(index)) return lowResStructures;

        //Si no es ninguna de las anteriores, es cualquier obst�culo "normal"
        return obstaclesStructures; 
    }

    //Genera los index de los beats en los que va a haber powerUps
    private void GeneratePowerUpsIndexes(List<float> beats) 
    { 
        int margin = 15; //Ni los primeros "margin" beats ni los �ltimos habr� power ups

        bool indexValido = false;

        //GRAVITY POWER UP
        int gravityZoneLength = Random.Range(10, 30); //La zona con gravedad cambiada ser�n entre 10 y 30 beats

        while (!indexValido)
        {
            gravityStartIndex = Random.Range(margin, beats.Count - margin - gravityZoneLength);
            gravityEndIndex = gravityStartIndex + gravityZoneLength;

            indexValido = (!importantIndexes.Contains(gravityStartIndex)) && (!importantIndexes.Contains(gravityEndIndex));
        }
        

        importantIndexes.Add(gravityStartIndex);
        importantIndexes.Add(gravityEndIndex);
        Debug.Log("El comienzo de la zona Gravedad est� creado en el beat " + gravityStartIndex);
        Debug.Log("El final de la zona Gravedad est� creado en el beat " + gravityEndIndex);


        //SLOW MOTION POWER UP
        slowMotionIndexes = new List<int>();

        int numSlowMotions = Random.Range(1, 4); //Va a haber entre 1 y 3 (el 4 es exclu�do) slowMotions
        int marginBetweenSMPowerUps = 25; //M�nimo tiene que haber 15 beats entre un SlowMotion y otro

        for (int i = 0; i < numSlowMotions; ++i) //Para cada slowMotion que se vaya a crear
        {
            //Index v�lido: no seleccionado por otro powerUp anterior (gravity) y con marginBetweenBQPowerUps entre los de su propio tipo
            int newSlowMotionIndex = 0;
            indexValido = false;
            while (!indexValido)
            {
                newSlowMotionIndex = Random.Range(margin, beats.Count - 1 - margin);

                //Esto indica si este index es v�lido comprobando que no se haya usado en otro powerUp anterior (gravity)
                indexValido = !importantIndexes.Contains(newSlowMotionIndex);

                //A�n falta comprobar la separacion entre los powerUps del mismo tipo (si i = 0 no hace falta)
                if (i > 0)
                {
                    int j = 0;
                    while (indexValido && j < i) //Mientras siga siendo v�lido, seguimos comprobando (si no es v�lido ya no hace falta comprobar)
                    {
                        indexValido = newSlowMotionIndex > slowMotionIndexes[j] + marginBetweenSMPowerUps || newSlowMotionIndex < slowMotionIndexes[j] - marginBetweenSMPowerUps;
                        j++;
                    }
                }
            }
            
            slowMotionIndexes.Add(newSlowMotionIndex);
            importantIndexes.Add(newSlowMotionIndex);

            Debug.Log("Power Up de SlowMotion creado en el beat " + newSlowMotionIndex);
        }


        //LOW RES POWER UP
        lowResIndexes = new List<int>();

        int numLowRes = Random.Range(1, 3); //Va a haber entre 1 y 2 (el 3 es exclu�do) LowRes
        int marginBetweenBQPowerUps = 50; //M�nimo tiene que haber 30 beats entre un SlowMotion y otro

        for (int i = 0; i < numLowRes; ++i) //Para cada LowRes que se vaya a crear
        {
            //Index v�lido: no seleccionado por otro powerUp anterior (gravity y SlowMotion) y con marginBetweenBQPowerUps entre los de su propio tipo
            int newLowResIndex = 0;
            indexValido = false;
            while(!indexValido)
            {
                newLowResIndex = Random.Range(margin, beats.Count - 1 - margin);

                //Esto indica si este index es v�lido comprobando que no se haya usado en otro powerUp anterior (gravity y slowMotion)
                indexValido = !importantIndexes.Contains(newLowResIndex);

                //A�n falta comprobar la separacion entre los powerUps del mismo tipo (si i = 0 no hace falta)
                if (i > 0)
                {
                    int j = 0;
                    while (indexValido && j < i) //Mientras siga siendo v�lido, seguimos comprobando (si no es v�lido ya no hace falta comprobar)
                    {
                        indexValido = newLowResIndex > lowResIndexes[j] + marginBetweenBQPowerUps || newLowResIndex < lowResIndexes[j] - marginBetweenBQPowerUps;
                        j++;
                    }
                }
            }
            
            lowResIndexes.Add(newLowResIndex);

            Debug.Log("Power Up de LowRes creado en el beat " + newLowResIndex);
        }
    }

    private void InitZonesIndexes()
    {
        foreach (ZoneData z in zonesData)
        {
            if (z.getType() == ZoneType.LOW)
            {
                lowZoneStartIndex = z.getBeatIni();
                lowZoneEndIndex = z.getBeatEnd();
            }
        }

        importantIndexes.Add(lowZoneStartIndex);
        importantIndexes.Add(lowZoneEndIndex);
    }

}