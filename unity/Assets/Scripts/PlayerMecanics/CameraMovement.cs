using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTr;
    [SerializeField] private float offsetX, offsetY, maxSpeed;
    float posZ;
    private Quaternion newCameraRotation;
    private bool putamierda;

    void Start()
    {
        posZ = transform.position.z;
        newCameraRotation = new Quaternion(0, 0, 0, 0);
        putamierda = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) newCameraRotation = Quaternion.Euler(0, 0, -180);
        if (Input.GetKeyDown(KeyCode.O)) newCameraRotation = Quaternion.Euler(0, 0, 0);
        //Debug.Log(newCameraRotation+  "  " + transform.localRotation);
        if (Mathf.Abs(transform.position.y - playerTr.position.y) > offsetY)
            transform.position = new Vector3(playerTr.position.x + offsetX, Mathf.Lerp(transform.position.y, playerTr.position.y, Time.deltaTime * maxSpeed), posZ);
        else transform.position = new Vector3(playerTr.position.x + offsetX, transform.position.y, posZ);


        //if (transform.rotation.eulerAngles != newCameraRotation.eulerAngles)
        //{
        //    Debug.Log(transform.rotation.eulerAngles.z + "AAAA" + newCameraRotation.eulerAngles.z);
        //    transform.rotation = Quaternion.Euler(0, 0, Mathf.MoveTowards(transform.rotation.eulerAngles.z, newCameraRotation.eulerAngles.z, Time.deltaTime*180.0f*2));
        //}

    }
}