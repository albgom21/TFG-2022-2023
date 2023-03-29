using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoJump : MonoBehaviour
{
    private bool jump;
    private PlayerMovement playerMov;
    private Rigidbody2D rb;
    private int ticks; //JAJA XD LOL LOL LOL LOL LOL LOL LOL MIRA ESTO LOL
    private void Start()
    {
        jump = false;
        rb = null;
        ticks = 0;
    }
    private void FixedUpdate()
    {
        if (jump)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * 26.6581f, ForceMode2D.Impulse);
            ticks++;

            jump = ticks < 2;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMov = collision.gameObject.GetComponent<PlayerMovement>();

        if (playerMov != null)
        {
            rb = collision.gameObject.GetComponent<Rigidbody2D>();
            jump = true;
            ticks = 0;
        }
    }
}
