using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform playerTr;
    public float offsetX, offsetY;
    float posY, posZ;

    void Start()
    {
        posY = transform.position.y;
        posZ = transform.position.z;
    }

    void Update()
    {
        transform.position = new Vector3(playerTr.position.x + offsetX, posY/*playerTr.position.y + offsetY*/, posZ);
    }
}
