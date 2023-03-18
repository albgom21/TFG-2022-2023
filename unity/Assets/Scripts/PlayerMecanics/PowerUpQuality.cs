using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpQuality : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            GameManager.instance.setPowerUpQuality(true);
            Destroy(gameObject);
        }
    }
}
