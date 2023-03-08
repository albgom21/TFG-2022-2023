using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float m;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("algo sa chocao");
        PlayerMovement mov = collision.gameObject.GetComponent<PlayerMovement>();
        if (mov != null)
        {
            Debug.Log("salto gooordo");
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * 26.6581f*m, ForceMode2D.Impulse);
        }
    }
}