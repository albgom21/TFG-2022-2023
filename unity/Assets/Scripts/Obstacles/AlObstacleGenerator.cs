using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.IO;
//using UnityEditor.Experimental.GraphView;
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
    private GameObject[] highZoneStartStructures;
    private GameObject[] highZoneEndStructures;

    //Estructuras que contengan POWER UPS
    private GameObject[] gravityStructures;
    private GameObject[] slowMotionStructures;
    private GameObject[] lowResStructures;

    private int gravityStartIndex, gravityEndIndex; //Va a haber SIEMPRE solo dos power ups de Gravedad (uno que activa y otro desactiva)
    private List<int> slowMotionIndexes; //Lista de index donde se crearán los powerUps de slowMotion
    private List<int> lowResIndexes; //Lista de index donde se crearán los powerUps de lowRes

    private List<int> importantIndexes; //Lista de index con TODOS los structs especiales (powerUps, cambios de zona etc)

    [SerializeField] private GameObject endPrefab;

    // Pools
    [SerializeField] private Transform obstaclePool;
    [SerializeField] private Transform groundPool;

    // Multipliers
    private float multiplierX;

    private ObstacleStructureData lastObstacle;

    // Dificultad
    private int difficulty;

    // Zones
    List<ZoneData> zonesData;
    Color lowColor, highColor;
    bool lowColorChange = false;
    bool highColorChange = false;

    private int lowZoneStartIndex, lowZoneEndIndex;
    private int highZoneStartIndex, highZoneEndIndex;


    //Guardado / Cargado de niveles
    [SerializeField] private bool saveOrLoadLevel;
    string rutaNivelCreado;
    List<int> obstacleIndexes; //Índices de los obstáculos en orden para el guardado del nivel

    void Start()
    {
        lowColor = new Color(0.0f, 1.0f, 0.6344354f, 1.0f);  // GREEN-BLUE
        highColor = new Color(1.0f, 0.2731888f, 0.25f, 1.0f); // RED

        InitResources();

        lastObstacle = null;

        List<float> beats = features.GetComponent<ReadTxt>().GetBeatsInTime();
        zonesData = zones.getZonesData();

        List<float> rmse = features.GetComponent<ReadTxt>().GetRMSE();

        multiplierX = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().GetPlayerSpeed();

        if (saveOrLoadLevel && LevelIsAlreadyCreated())
        {
            LoadLevel();
            LoadObstacles(beats);
        }
        else
        {
            InitIndexes(beats);
            GenerateObstacles(beats, rmse);
            if (saveOrLoadLevel) SaveLevel();
        }
        
        PositionPlayer();
    }

    private void GenerateObstacles(List<float> beats, List<float> rmse)
    {
        //Coordenadas del obstáculo y del previo
        float coordX, coordY, prevX;
        coordX = coordY = prevX = 0.0f;

        ObstacleStructureData thisObstacle = null;
        obstacleIndexes = new List<int>();

        for (int i = 0; i < beats.Count(); i++)
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

                //Si este index es justo el anterior a donde va a haber uno importante, quiero instanciar uno vacío (Default).
                if (importantIndexes.Contains(i + 1)) thisObstacle = InstantiateDefaultObstacle(getPosibleStructures(i), coordX, coordY);

                else //Si no, uno aleatorio con la dificultad correspondiente
                {
                    difficulty = ChooseDifficulty(rmse[i]); //Decisión de dificultad
                    thisObstacle = InstantiateRandomObstacle(getPosibleStructures(i), coordX, coordY, spaceBetweenBeats);
                }


                //CREACIÓN DEL SUELO
                //El suelo tendrá que ir desde el FINAL (no el centro) del anterior obstáculo hasta el PRINCIPIO de este
                float floorStart, floorEnd;

                if (lastObstacle == null) floorStart = -2.0f * multiplierX; //Solo ocurre con el primer obstáculo, el cual no tiene anterior
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
                thisObstacle = null; //Si es el último, quiero que se guarde para la creación del END
            }
        }

        //END
        GenerateEnd();
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

            if (lastObstacle == null) correctObstacle = true; //Esto solo se da en el primer obstáculo, aún no hay anterior
            else correctObstacle = ObstacleEnabled(obstacleStructure) && (lastObstacle.getPostX() + obstacleStructure.getPrevX() < spaceBetweenBeats); //Comprueba si es correcto

            if (!correctObstacle)
            {
                Destroy(obstacle); //Si no lo es, lo destruye, volverá al while y creará otro.
                obstacleStructure = null;
            }
            else obstacleIndexes.Add(rnd);

            intentos++;
        }

        //Si ha llegado hasta aquí siendo null, significa que tras todos los intentos no ha encontrado ninguno que quepe.
        //Pongo A MANO el número 0 porque es el más "fino", cabe seguro (es un obstáculo vacío)
        if (obstacleStructure == null)
        {
            obstacleStructure = InstantiateDefaultObstacle(posibleStructures, x, y);
            //Debug.Log("Default instanciado por no encontrar");
        }

        return obstacleStructure;
    }

    //Instancia dentro de los posiblesStructures el por defecto (0) en las coordenadas x,y
    private ObstacleStructureData InstantiateDefaultObstacle(GameObject[] posibleStructures, float x, float y)
    {
        return InstantiateConcreteObstacle(posibleStructures, 0, x, y);
    }

    //Instancia dentro de los posiblesStructures el índice index en concreto en las coordenadas x,y
    private ObstacleStructureData InstantiateConcreteObstacle(GameObject[] posibleStructures, int index, float x, float y)
    {
        ChangeGroundColor(posibleStructures, index, lowColorChange, highColorChange);
        GameObject concreteObstacle = Instantiate(posibleStructures[index], new Vector3(x, y, 0), transform.rotation, obstaclePool);
        obstacleIndexes.Add(0);
        return concreteObstacle.GetComponent<ObstacleStructureData>();
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

    //Genera un suelo en la altura y entre las coordenadasX floorStart y floorEnd
    private void GenerateFloor(float floorStart, float floorEnd, float y)
    {
        float width = floorEnd - floorStart;
        GameObject ground = Instantiate(groundPrefab, new Vector3(floorStart + (width / 2.0f), y, 0), transform.rotation, groundPool);
        ground.transform.localScale = new Vector3(width, ground.transform.localScale.y, ground.transform.localScale.z);
    }

    //Genera el final del nivel (suelo desde el último obstáculo hasta el final del .mp3, donde se generará el endPrefab)
    private void GenerateEnd()
    {
        //La coordenada X del suelo va desde EL FINAL del último obstáculo hasta que se acabe el .mp3
        float floorStartX = lastObstacle.gameObject.transform.position.x + lastObstacle.getPostX();
        float floorEndX = features.GetComponent<ReadTxt>().GetDuration() * multiplierX;

        //La coordenada y tanto del suelo como del endPrefab será la del último obstáculo + su desnivel
        float coordY = lastObstacle.gameObject.transform.position.y + lastObstacle.getUnlevel();

        GenerateFloor(floorStartX, floorEndX, coordY);

        //El endPrefab se generá tras el floor en la misma coorY
        float coordX = floorEndX + endPrefab.transform.localScale.x / 2.0f;

        Instantiate(endPrefab, new Vector3(coordX, coordY, 0), transform.rotation, obstaclePool);
    }

    public float getMultiplierX() { return multiplierX; }
    public GameObject getFeatures() { return features;  }


    //Devuelve si el obstáculo elegido está activado para usarlo y si es posible por su dificultad
    private bool ObstacleEnabled(ObstacleStructureData obstacleStructure)
    {
        int obstacleDif = obstacleStructure.getDifficulty();

        //Si la dificultad del obstáculo es -1, me da igual la dificultad actual (el obstáculo se puede usar en cualquier dificultad)
        if (obstacleDif == -1) return obstacleStructure.getObstacleEnabled(); 

        //Devuelve si el obstáculo está activado y su dificultad es la correcta
        return obstacleStructure.getObstacleEnabled() && difficulty == obstacleDif;
    }

    //Devuelve el array de estructuras que se va a utilizar para el siguiente beat
    private GameObject[] getPosibleStructures(int index)
    {
        //---------ESTRUCTURAS DE ZONAS-------------------------
        //Estructuras de comienzo de zonaLow (agua)
        if (index == lowZoneStartIndex) return lowZoneStartStructures;

        //Estructuras de final de zonaLow (agua)
        if (index == lowZoneEndIndex) return lowZoneEndStructures;

        //Estructuras de comienzo de zonaHigh
        if (index == highZoneStartIndex) return highZoneStartStructures;

        //Estructuras de final de zonaHigh
        if (index == highZoneEndIndex) return highZoneEndStructures;

        //---------ESTRUCTURAS DE POWER UPS-------------------------
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
        int margin = 15; //Ni los primeros "margin" beats ni los últimos habrá power ups

        bool indexValido = false;

        //GRAVITY POWER UP
        int gravityZoneLength = Random.Range(10, 30); //La zona con gravedad cambiada serán entre 10 y 30 beats

        while (!indexValido)
        {
            gravityStartIndex = Random.Range(margin, beats.Count - margin - gravityZoneLength);
            gravityEndIndex = gravityStartIndex + gravityZoneLength;

            indexValido = (!importantIndexes.Contains(gravityStartIndex)) && (!importantIndexes.Contains(gravityEndIndex));
        }
        

        importantIndexes.Add(gravityStartIndex);
        importantIndexes.Add(gravityEndIndex);
        Debug.Log("El comienzo de la zona Gravedad está creado en el beat " + gravityStartIndex);
        Debug.Log("El final de la zona Gravedad está creado en el beat " + gravityEndIndex);


        //SLOW MOTION POWER UP
        slowMotionIndexes = new List<int>();

        int numSlowMotions = Random.Range(1, 4); //Va a haber entre 1 y 3 (el 4 es excluído) slowMotions
        int marginBetweenSMPowerUps = 25; //Mínimo tiene que haber 15 beats entre un SlowMotion y otro

        for (int i = 0; i < numSlowMotions; ++i) //Para cada slowMotion que se vaya a crear
        {
            //Index válido: no seleccionado por otro powerUp anterior (gravity) y con marginBetweenBQPowerUps entre los de su propio tipo
            int newSlowMotionIndex = 0;
            indexValido = false;
            while (!indexValido)
            {
                newSlowMotionIndex = Random.Range(margin, beats.Count - 1 - margin);

                //Esto indica si este index es válido comprobando que no se haya usado en otro powerUp anterior (gravity)
                indexValido = !importantIndexes.Contains(newSlowMotionIndex);

                //Aún falta comprobar la separacion entre los powerUps del mismo tipo (si i = 0 no hace falta)
                if (i > 0)
                {
                    int j = 0;
                    while (indexValido && j < i) //Mientras siga siendo válido, seguimos comprobando (si no es válido ya no hace falta comprobar)
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

        int numLowRes = Random.Range(1, 3); //Va a haber entre 1 y 2 (el 3 es excluído) LowRes
        int marginBetweenBQPowerUps = 50; //Mínimo tiene que haber 30 beats entre un SlowMotion y otro

        for (int i = 0; i < numLowRes; ++i) //Para cada LowRes que se vaya a crear
        {
            //Index válido: no seleccionado por otro powerUp anterior (gravity y SlowMotion) y con marginBetweenBQPowerUps entre los de su propio tipo
            int newLowResIndex = 0;
            indexValido = false;
            while(!indexValido)
            {
                newLowResIndex = Random.Range(margin, beats.Count - 1 - margin);

                //Esto indica si este index es válido comprobando que no se haya usado en otro powerUp anterior (gravity y slowMotion)
                indexValido = !importantIndexes.Contains(newLowResIndex);

                //Aún falta comprobar la separacion entre los powerUps del mismo tipo (si i = 0 no hace falta)
                if (i > 0)
                {
                    int j = 0;
                    while (indexValido && j < i) //Mientras siga siendo válido, seguimos comprobando (si no es válido ya no hace falta comprobar)
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

    //Posiciona al jugador
    private void PositionPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(-2.0f * multiplierX, player.transform.position.y, player.transform.position.z);
    }

    //Inicializa los arrays de estructuras
    private void InitResources()
    {
        obstaclesStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/Estructuras");
        lowZoneStartStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/ChangeZone/LowZoneStart");
        lowZoneEndStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/ChangeZone/LowZoneEnd");
        highZoneStartStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/ChangeZone/HighZoneStart");
        highZoneEndStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/ChangeZone/HighZoneEnd");
        gravityStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/Gravity");
        slowMotionStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/SlowMotion");
        lowResStructures = Resources.LoadAll<GameObject>("Prefabs/Alvaro/PowerUps/LowRes");
    }

    //Inicializa todos los Index (zonas y powerUps)
    private void InitIndexes(List <float> beats)
    {
        importantIndexes = new List<int>();
        InitZonesIndexes();
        GeneratePowerUpsIndexes(beats);

    }

    //Inicializa los Index de zonas
    private void InitZonesIndexes()
    {
        lowZoneStartIndex = lowZoneEndIndex = -1;
        highZoneStartIndex = highZoneEndIndex = -1;

        //DEBUG PARA VER LAS ESTRUCTURAS AL PRINCIPIO
        //lowZoneStartIndex = 3;
        //lowZoneEndIndex = 8;
        //highZoneStartIndex = 14;
        //highZoneEndIndex = 20;


        foreach (ZoneData z in zonesData)
        {
            if (z.getType() == ZoneType.LOW)
            {
                lowZoneStartIndex   = z.getBeatIni();
                lowZoneEndIndex     = z.getBeatEnd();
            }
            else if (z.getType() == ZoneType.HIGH)
            {
                highZoneStartIndex  = z.getBeatIni();
                highZoneEndIndex    = z.getBeatEnd();
            }
        }
        // Si hay zona low
        if(lowZoneStartIndex > 0 && lowZoneEndIndex > 0)
        {
            importantIndexes.Add(lowZoneStartIndex);
            importantIndexes.Add(lowZoneEndIndex);
        }
        // Si hay zona high
        if (highZoneStartIndex > 0 && highZoneEndIndex > 0)
        {
            importantIndexes.Add(highZoneStartIndex);
            importantIndexes.Add(highZoneEndIndex);
        }
    }

    //Cambia la dificultad dependiendo del rmse de este momento
    private int ChooseDifficulty(float rmse)
    {
        if (rmse < 0.6)     return 1;
        if (rmse < 0.8)     return 2;
        if (rmse < 0.9)     return 3;
        if (rmse < 0.98)    return 4;
        return 5; //Si rmse >= 0.98
    }

    private bool LevelIsAlreadyCreated()
    {
        string songName = GameManager.instance.GetSong();
        rutaNivelCreado = Application.streamingAssetsPath + "/" + songName  + "/" + songName + "_levelInfo.txt";
        return File.Exists(rutaNivelCreado);
    }

    private void SaveLevel()
    {
        //-------------------------GUARDADO DE LOS INDEXES DE LOS POWER-UPS---------------------------------------

        //GRAVITY
        File.AppendAllText(rutaNivelCreado, (gravityStartIndex.ToString() + "\n"));
        File.AppendAllText(rutaNivelCreado, (gravityEndIndex.ToString() + "\n"));

        //SLOWMOTION
        File.AppendAllText(rutaNivelCreado, (slowMotionIndexes.Count + "\n"));
        
        for(int i = 0; i < slowMotionIndexes.Count; ++i) File.AppendAllText(rutaNivelCreado, (slowMotionIndexes[i] + "\n"));

        //LOW RES
        File.AppendAllText(rutaNivelCreado, (lowResIndexes.Count + "\n"));

        for (int i = 0; i < lowResIndexes.Count; ++i) File.AppendAllText(rutaNivelCreado, (lowResIndexes[i] + "\n"));


        //------------------------GUARDADO DE LOS OBSTÁCULOS---------------------------------------------------

        for (int i = 0; i < obstacleIndexes.Count; ++i) File.AppendAllText(rutaNivelCreado, (obstacleIndexes[i] + "\n"));
    }

    private void LoadLevel()
    {
        string[] lines = File.ReadAllLines(rutaNivelCreado);
        int lineaActual = 0;
        obstacleIndexes = new List<int>();

        //--------------------------------------ZONAS-------------------------------------
        importantIndexes = new List<int>();
        InitZonesIndexes();

        //---------------------------CARGADO DE INDEXES DE POWER-UPS-------------------------------------

        //GRAVITY
        gravityStartIndex = int.Parse(lines[lineaActual]);
        importantIndexes.Add(gravityStartIndex);
        lineaActual++;

        gravityEndIndex = int.Parse(lines[lineaActual]);
        importantIndexes.Add(gravityEndIndex);
        lineaActual++;

        //SLOW MOTION
        slowMotionIndexes = new List<int>();

        int numSlowMotions = int.Parse(lines[lineaActual]);
        lineaActual++;

        for (int i = 0; i < numSlowMotions; ++i)
        {
            int newIndex = int.Parse(lines[lineaActual]);
            slowMotionIndexes.Add(newIndex);
            importantIndexes.Add(newIndex);
            lineaActual++;
        }

        //LOW RES
        lowResIndexes = new List<int>();

        int numLowRes = int.Parse(lines[lineaActual]);
        lineaActual++;

        for (int i = 0; i < numLowRes; ++i)
        {
            int newIndex = int.Parse(lines[lineaActual]);
            lowResIndexes.Add(newIndex);
            importantIndexes.Add(newIndex);
            lineaActual++;
        }

        //--------------------------------CARGADO DE OBSTÁCULOS-------------------------------------
        for (int i = lineaActual; i < lines.Length; ++i)
        {
            obstacleIndexes.Add(int.Parse(lines[i]));
        }
    }

    private void LoadObstacles(List<float> beats)
    {
        //Coordenadas del obstáculo y del previo
        float coordX, coordY, prevX;
        coordX = coordY = prevX = 0.0f;

        ObstacleStructureData thisObstacle = null;
        int thisObstacleIndex = 0;

        for (int i = 0; i < beats.Count(); i++)
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
                thisObstacle = InstantiateConcreteObstacle(getPosibleStructures(i), obstacleIndexes[thisObstacleIndex], coordX, coordY);
                thisObstacleIndex++;


                //CREACIÓN DEL SUELO
                //El suelo tendrá que ir desde el FINAL (no el centro) del anterior obstáculo hasta el PRINCIPIO de este
                float floorStart, floorEnd;

                if (lastObstacle == null) floorStart = -2.0f * multiplierX; //Solo ocurre con el primer obstáculo, el cual no tiene anterior
                else floorStart = prevX + lastObstacle.getPostX();

                floorEnd = coordX - thisObstacle.getPrevX();

                GenerateFloor(floorStart, floorEnd, coordY);
            }

            //Preparando la siguiente iteración
            if (thisObstacle != null)
            {
                prevX = coordX;
                coordY += thisObstacle.getUnlevel();
                lastObstacle = thisObstacle;
                thisObstacle = null; //Si es el último, quiero que se guarde para la creación del END
            }
        }

        //END
        GenerateEnd();
    }
}