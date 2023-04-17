using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TORCH START");
        GameManager.instance.AddTorchGM(this.gameObject);
    }
}
