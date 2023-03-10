using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform sprite;
    [SerializeField] private GameObject obstacleGenerator;
    [SerializeField] private GameObject spawnPrefab;
    private Rigidbody2D rb;
    private bool jump, onGround;

    void Start()
    {
        int startingY = (int)(obstacleGenerator.GetComponent<ObstacleGenerator>().getFeatures().GetComponent<ReadTxt>().getScopt()[0] * obstacleGenerator.GetComponent<ObstacleGenerator>().getMultiplierY());
        rb = GetComponent<Rigidbody2D>();
        transform.SetPositionAndRotation(new Vector3(transform.position.x, startingY, transform.position.z), transform.rotation);
        jump = false; onGround = true;
        GameManager.instance.setStartPosition(transform.position);
        Instantiate(spawnPrefab, transform.position, transform.rotation);
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
        if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space)) jump = true;
        else jump = false;
        if (onGround) Unrotate();
        else sprite.Rotate(Vector3.back * 1.5f);
    }

    private void FixedUpdate()
    {
        if (onGround && jump)
        {
            //Jump
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * 26.6581f, ForceMode2D.Impulse);
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        if (normal == Vector2.up) onGround = true;
        else if (normal == Vector2.down || normal == Vector2.left) playerDeath();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
    }

    public void playerDeath()
    {
        GameManager.instance.setDeath(true);
        transform.position = GameManager.instance.getStartPosition();
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<RestartMusic>().restartMusic();
    }
}