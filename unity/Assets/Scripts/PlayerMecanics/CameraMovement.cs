using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTr;
    [SerializeField] private float offsetX, offsetY;
    float posZ;
    bool moveCam;

    void Start()
    {
        posZ = transform.position.z;
    }

    void Update()
    {
        float dif = Mathf.Abs(transform.position.y - playerTr.position.y);
        if (dif > offsetY && !moveCam) moveCam = true;
        if (moveCam)
        {
            transform.position = new Vector3(playerTr.position.x + offsetX, Mathf.Lerp(transform.position.y, playerTr.position.y,
                    Time.deltaTime * dif/2.0f), posZ);
            if (dif<1) moveCam = false;
        }
        else transform.position = new Vector3(playerTr.position.x + offsetX, transform.position.y, posZ);

    }
}