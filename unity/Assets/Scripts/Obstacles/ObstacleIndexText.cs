using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleIndexText : MonoBehaviour
{
    private DebugManager debugManager;
    void Start()
    {
        debugManager = GameManager.instance.GetDebugManager();
        debugManager.addObstacleIndexTextInstance(this.gameObject);
        this.GetComponent<MeshRenderer>().enabled = debugManager.getDebugMode();
    }
}