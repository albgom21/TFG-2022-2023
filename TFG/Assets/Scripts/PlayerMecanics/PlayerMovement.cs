using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform sprite;
    [SerializeField] private GameObject obstacleGenerator;
    private Rigidbody2D rb;

    void Start()
    {
        int startingY = (int)(obstacleGenerator.GetComponent<ObstacleGenerator>().getFeatures().GetComponent<ReadTxt>().getScopt()[0] * obstacleGenerator.GetComponent<ObstacleGenerator>().getMultiplierY() - 1);
        rb = GetComponent<Rigidbody2D>();
        transform.SetPositionAndRotation(new Vector3(transform.position.x, startingY, transform.position.z), transform.rotation);
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
        else sprite.Rotate(Vector3.back * 2);
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

    public float getPlayerSpeed()
    {
        return speed;
    }
}
