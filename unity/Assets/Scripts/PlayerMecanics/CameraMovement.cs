using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform playerTr;
    public float offsetX, offsetY;
    float posY, posZ;
    // Start is called before the first frame update
    void Start()
    {
        posY = transform.position.y;
        posZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerTr.position.x + offsetX, posY + offsetY, posZ);
    }
}
