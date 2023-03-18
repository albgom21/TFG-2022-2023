using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpGravity : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            Transform tr = GameObject.FindWithTag("RawImage").GetComponent<Transform>();
            if (GameManager.instance.getPowerUpGravity())
            {
                tr.localRotation = new Quaternion(0, 0, 0, 0);
                GameManager.instance.setPowerUpGravity(false);
            }
            else
            {
                tr.localRotation = new Quaternion(-180, 0, 0, 0);
                GameManager.instance.setPowerUpGravity(true);
            }

            Destroy(gameObject);
        }
    }
}
