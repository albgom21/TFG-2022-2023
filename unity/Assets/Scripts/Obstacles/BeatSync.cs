using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSync : MonoBehaviour
{
    private DebugManager debugManager;
    void Start()
    {
        debugManager = GameManager.instance.GetDebugManager();
        debugManager.addSyncInstance(this.gameObject);
        this.GetComponent<MeshRenderer>().enabled = debugManager.getDebugMode();
    }
}