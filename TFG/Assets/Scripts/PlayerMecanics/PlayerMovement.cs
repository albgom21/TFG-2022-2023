using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    public Transform groundCheckTransform;
    public float groundCheckRadius;
    public LayerMask groundMask;
    public Transform sprite;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (OnGround())
        {
            Unrotate();

            //Jump
            if (Input.GetMouseButton(0))
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.up * 26.6581f, ForceMode2D.Impulse);
            }
        }
        else
        {
            sprite.Rotate(Vector3.back * 2);
        }
    }


    bool OnGround()
    {
        return Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundMask);
    }

    void Unrotate()
    {
        Vector3 rotation = sprite.rotation.eulerAngles;
        rotation.z = Mathf.Round(rotation.z / 90) * 90;
        sprite.rotation = Quaternion.Euler(rotation);
    }

    
}
