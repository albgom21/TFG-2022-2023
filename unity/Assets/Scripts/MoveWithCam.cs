using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithCam : MonoBehaviour
{ 
    [SerializeField]
    private Camera cam;
    
    void Update()
    {
        transform.position = new Vector3(cam.transform.position.x, transform.position.y, transform.position.z);
    }
}
