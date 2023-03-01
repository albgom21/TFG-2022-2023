using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Transform playerTr;
    Vector3 originalPosition;

    void Start()
    {
        originalPosition = playerTr.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7) //Capa de enemigos
        {
            playerTr.position = originalPosition;
            GetComponentInParent<RestartMusic>().restartMusic();
        }
    }
}
