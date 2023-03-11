using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private bool interactive;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement mov = collision.gameObject.GetComponent<PlayerMovement>();
        if (mov != null)
        {
            if (interactive && !(Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space))) return;
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * 26.6581f * jumpForce, ForceMode2D.Impulse);
        }
    }
}
