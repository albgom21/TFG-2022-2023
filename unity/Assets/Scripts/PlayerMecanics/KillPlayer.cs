using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
        if (pm != null) pm.PlayerDeath();
    }
}
