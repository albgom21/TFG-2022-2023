using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoJump : MonoBehaviour
{
    private float jumpForce;
    private Rigidbody2D rb;
    private AutoJumpManager autoJumpManager;
    private DebugManager debugManager;

    private void Start()
    {
        rb = null;
        jumpForce = 0;

        //DebugMode
        debugManager = GameManager.instance.getDebugManager();
        debugManager.addAutoJumpInstance(this.gameObject);
        this.GetComponent<SpriteRenderer>().enabled = debugManager.getDebugMode();

        //AutoJumpMode
        autoJumpManager = GameManager.instance.getAutoJumpManager();
        autoJumpManager.AddInstance(this.gameObject);
        bool startEnabled = autoJumpManager.GetAutoJumpEnabled();
        if (startEnabled) GetComponent<SpriteRenderer>().color = Color.green;
        else GetComponent<SpriteRenderer>().color = Color.red;
        this.enabled = startEnabled;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMov = collision.gameObject.GetComponent<PlayerMovement>();

        if (playerMov != null)
        {
            playerMov.AutoJump();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else Debug.LogError("RigidBody2D not found in AutoJump Script");

        }
    }

    public void SetPlayerVariables(GameObject p)
    {
        rb = p.GetComponent<Rigidbody2D>();
        jumpForce = p.GetComponent<PlayerMovement>().GetJumpForce();
    }
}