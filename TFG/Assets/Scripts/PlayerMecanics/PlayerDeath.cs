using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Transform playerTr;
    Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    { 
        originalPosition = playerTr.position;
    }

    // Update is called once per frame


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7) playerTr.position = originalPosition;
    }
}
