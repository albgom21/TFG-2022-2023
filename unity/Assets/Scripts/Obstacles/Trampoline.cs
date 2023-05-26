using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private bool interactive;
    private PlayerMovement playerMov;
    private bool alreadyJumped = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerMov = collision.gameObject.GetComponent<PlayerMovement>();
        alreadyJumped = false;

        TrampolineJump();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TrampolineJump();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        alreadyJumped = false;
    }

    private void TrampolineJump()
    {
        if (playerMov != null && !alreadyJumped)
        {
            bool wantToJump = playerMov.GetAutoJump() || Input.GetMouseButton(0);

            if (!interactive || wantToJump)
            {
                playerMov.Jump(jumpForce);
                alreadyJumped = true;
            }
        }
    }
}
