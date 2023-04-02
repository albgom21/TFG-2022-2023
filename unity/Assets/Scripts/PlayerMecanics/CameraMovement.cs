using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTr;
    [SerializeField] private float offsetX, offsetY;
    float posZ;

    void Start()
    {
        posZ = transform.position.z;
    }

    void Update()
    {
        if (Mathf.Abs(transform.position.y - playerTr.position.y) > offsetY)
            transform.position = new Vector3(playerTr.position.x + offsetX, Mathf.Lerp(transform.position.y, playerTr.position.y, Time.deltaTime), posZ);
        else transform.position = new Vector3(playerTr.position.x + offsetX, transform.position.y, posZ);
    }
}