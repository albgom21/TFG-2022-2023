using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed, jumpForce;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform sprite;
    [SerializeField] private GameObject obstacleGenerator, spawnPrefab, spawnPool;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Crono crono;
    [SerializeField] private float maxFallingSpeed;
    private Rigidbody2D rb;
    private bool autoJump, jump, onGround, createSpawn;
    private PowerUpsManager powerUpsManager;
    private struct SpawnData
    {
        public Vector3 pos;
        public GameObject obj;
        public double time;
        //Datos de los power Ups (así no hay que cambiar código en PlayerMovement cada vez que se añadan más powerUps)
        public PowerUpsManager.PowerUpsData powerUpsData;
        public SpawnData(Vector3 position, GameObject o, double t, PowerUpsManager.PowerUpsData puD)
        {
            pos = position;
            obj = o;
            time = t;
            powerUpsData = puD;
        }
    }
    List<SpawnData> spawns = new();

    private void Awake()
    {
        GameManager.instance.SetDeath(false);
        GameManager.instance.SetEnd(false);
    }

    void Start()
    {
        powerUpsManager = GameManager.instance.GetPowerUpsManager();
        rb = GetComponent<Rigidbody2D>();
        jump = autoJump = createSpawn = false; onGround = true;
        spawns.Add(new SpawnData(transform.position, Instantiate(spawnPrefab, transform.position, transform.rotation, spawnPool.transform), 0,
            new PowerUpsManager.PowerUpsData()));
    }

    void Update()
    {
        if (GameManager.instance.GetDeath()) GameManager.instance.SetDeath(false);

        transform.position += Vector3.right * speed * Time.deltaTime;
        if (Input.GetMouseButton(0)) jump = true;
        else if (!autoJump) jump = false;

        if (Input.GetMouseButtonDown(1)) createSpawn = true;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (spawns.Count > 1)
            {
                var obj = spawns[spawns.Count - 1].obj;
                Destroy(obj);
                spawns.RemoveAt(spawns.Count - 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = spawns.Count - 1; i > 0; i--)
            {
                var obj = spawns[i].obj;
                Destroy(obj);
                spawns.RemoveAt(i);
            }
            PlayerDeath();
        }

        if (Input.GetKeyDown(KeyCode.J)) GameManager.instance.ChangeAutoJumpMode();
        if (Input.GetKeyDown(KeyCode.D)) GameManager.instance.ChangeDebugMode();
        if (onGround)
        {
            Unrotate();
            particles.enableEmission = true;
        }
        else
        {
            sprite.Rotate(Vector3.back * 360.0f * Time.deltaTime); // ROTACIÓN
            particles.enableEmission = false;
        }
        if (Input.GetKeyDown(KeyCode.M)) PlayerDeath();

        if (createSpawn && onGround)
        {
            spawns.Add(new SpawnData(transform.position,
                                    Instantiate(spawnPrefab, transform.position, transform.rotation, spawnPool.transform),
                                    crono.getActualTime(),
                                    powerUpsManager.getData() //Info de los powerUps
                                    ));
            createSpawn = false;
        }
    }

    private void FixedUpdate()
    {
        if (jump) TryJump();
        if (rb.velocity.y < 0) onGround = false;
        if (rb.velocity.y < -maxFallingSpeed) rb.velocity = new Vector2(rb.velocity.x, -maxFallingSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        if (normal == Vector2.down && rb.velocity.y > 0) PlayerDeath();
        else if (normal == Vector2.up) onGround = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        if (normal == Vector2.left)
        {
            float offset = Math.Abs(transform.position.y - collision.transform.position.y);
            if (offset < 0.9f) PlayerDeath();
        }
    }

    void Unrotate()
    {
        Vector3 rotation = sprite.rotation.eulerAngles;
        rotation.z = Mathf.Round(rotation.z / 90) * 90;
        sprite.rotation = Quaternion.Euler(rotation);
    }

    public void PlayerDeath()
    {
        onGround = true; jump = false; autoJump = false; createSpawn = false;

        GameManager.instance.SetDeath(true);
        SpawnData lastSpawn = spawns[spawns.Count - 1];
        
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        transform.position = lastSpawn.pos;

        GameManager.instance.SetDeathTime(lastSpawn.time);
        crono.setActualTime(lastSpawn.time);
        GetComponent<ControlMusic>().ResetMusic((int)(lastSpawn.time * 1000.0));

        //Resetear el estado de los powerUps
        powerUpsManager.ResetData(lastSpawn.powerUpsData);
    }
    
    //Intenta saltar (comprobando si estás en el suelo)
    private void TryJump()
    {
        if (onGround) Jump();
    }

    //Saltas con una potencia dada (para trampolines)
    public void Jump(float jumpForceMultiplier = 1.0f)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(jumpForce * jumpForceMultiplier * Vector2.up, ForceMode2D.Impulse);
        autoJump = jump = onGround = false;
    }
    internal void AutoJump() { jump = true; autoJump = true; }

    public bool GetAutoJump() { return autoJump; }
    public float GetPlayerSpeed() { return speed; }

    public void CreateRespawn() { createSpawn = true; }
}