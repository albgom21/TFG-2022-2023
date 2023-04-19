using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float jumpForce;
    //[SerializeField] private float jumpForce;
    [SerializeField] private bool interactive;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement mov = collision.gameObject.GetComponent<PlayerMovement>();
        if (mov != null)
        {
            if (interactive && !(Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space))) return;
            mov.Jump(jumpForce);
        }
    }

    public void setJumpForce(int newJF) { jumpForce = newJF; }
}
