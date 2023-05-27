using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTr;
    [SerializeField] private float offsetX, offsetY, maxSpeed;
    float posZ;

    void Start()
    {
        posZ = transform.position.z;
    }

    void Update()
    {
        //Debug.Log(newCameraRotation+  "  " + transform.localRotation);
        if (Mathf.Abs(transform.position.y - playerTr.position.y) > offsetY)
            transform.position = new Vector3(playerTr.position.x + offsetX, Mathf.Lerp(transform.position.y, playerTr.position.y, Time.deltaTime * maxSpeed), posZ);
        else transform.position = new Vector3(playerTr.position.x + offsetX, transform.position.y, posZ);

    }
}