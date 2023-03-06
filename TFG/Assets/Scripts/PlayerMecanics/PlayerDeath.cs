using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7) //Capa de enemigos
        {
            transform.position = GameManager.instance_.getStartPosition();
            GetComponentInParent<RestartMusic>().restartMusic();
        }
    }
}
