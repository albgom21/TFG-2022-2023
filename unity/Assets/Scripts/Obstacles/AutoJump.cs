using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoJump : MonoBehaviour
{
    private bool jump;
    private Rigidbody2D rb;
    private int ticks; //JAJA XD LOL LOL LOL LOL LOL LOL LOL MIRA ESTO LOL
    private AutoJumpManager autoJumpManager;
    private DebugManager debugManager;

    private void Start()
    {
        jump = false;
        rb = null;
        ticks = 0;

        //DebugMode
        debugManager = GameManager.instance.getDebugManager();
        debugManager.addAutoJumpInstance(this.gameObject);
        this.GetComponent<SpriteRenderer>().enabled = debugManager.getDebugMode();

        //AutoJumpMode
        autoJumpManager = GameManager.instance.getAutoJumpManager();
        autoJumpManager.addInstance(this.gameObject);
        bool startEnabled = autoJumpManager.getAutoJumpEnabled();
        if (startEnabled) GetComponent<SpriteRenderer>().color = Color.green;
        else            GetComponent<SpriteRenderer>().color = Color.red;
        this.enabled = startEnabled;
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
