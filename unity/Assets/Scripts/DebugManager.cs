using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool debugMode;

    private List<GameObject> syncInstances;
    private List<GameObject> autoJumpInstances;
    private List<GameObject> obstacleIndexTextInstances;
    // Start is called before the first frame update
    void Start()
    {
        syncInstances = new List<GameObject>();
        autoJumpInstances = new List<GameObject>();
        obstacleIndexTextInstances = new List<GameObject>();
    }

    public bool getDebugMode() { return debugMode; }

    public void changeDebugMode()
    {
        debugMode = !debugMode;
        updateInstances();
    }

    public void updateInstances()
    {
        for (int i = 0; i < autoJumpInstances.Count; ++i)
        {
            autoJumpInstances[i].GetComponent<SpriteRenderer>().enabled = debugMode;
        }

        for (int j = 0; j < syncInstances.Count; ++j)
        {
            syncInstances[j].GetComponent<MeshRenderer>().enabled = debugMode;
        }

        for (int k = 0; k < syncInstances.Count; ++k)
        {
            obstacleIndexTextInstances[k].GetComponent<MeshRenderer>().enabled = debugMode;
        }
    }

    public void addSyncInstance(GameObject newSyncInstance) { syncInstances.Add(newSyncInstance); }

    public void addAutoJumpInstance(GameObject newAutoJump) { autoJumpInstances.Add(newAutoJump); }

    public void addObstacleIndexTextInstance(GameObject newAutoJump) { obstacleIndexTextInstances.Add(newAutoJump); }

}
