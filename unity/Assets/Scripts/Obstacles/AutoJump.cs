using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoJump : MonoBehaviour
{
    private AutoJumpManager autoJumpManager;
    private DebugManager debugManager;

    private void Start()
    {

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

        if (playerMov != null) playerMov.AutoJump();
    }
}