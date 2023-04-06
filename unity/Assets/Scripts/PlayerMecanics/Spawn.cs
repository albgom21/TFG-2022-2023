using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement mov = collision.gameObject.GetComponent<PlayerMovement>();
        if (mov != null)
            GameManager.instance.SetDeath(false);
    }
}