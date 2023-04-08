using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform sprite;
    [SerializeField] private GameObject obstacleGenerator;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private GameObject spawnPool;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Crono crono;
    private Rigidbody2D rb;
    private bool autoJump, jump, onGround;
    private PowerUpsManager powerUpsManager;
    private struct spawnData
    {
        public Vector3 pos;
        public GameObject obj;
        public double time;
        //Datos de los power Ups (así no hay que cambiar código en PlayerMovement cada vez que se añadan más powerUps)
        public PowerUpsManager.PowerUpsData powerUpsData;
        public spawnData(Vector3 position, GameObject o, double t, PowerUpsManager.PowerUpsData puD)
        {
            pos = position;
            obj = o;
            time = t;
            powerUpsData = puD;
        }
    }
    List<spawnData> spawns = new List<spawnData>();

    //private RaycastHit2D raycast;
    private float raycastDistance;

    private void Awake()
    {
        GameManager.instance.SetDeath(false);
        GameManager.instance.SetEnd(false);
    }

    void Start()
    {
        powerUpsManager = GameManager.instance.GetPowerUpsManager();
        int startingY = (int)(obstacleGenerator.GetComponent<ObstacleGenerator>().getFeatures().GetComponent<ReadTxt>().getScopt()[2] * obstacleGenerator.GetComponent<ObstacleGenerator>().getMultiplierY());
        rb = GetComponent<Rigidbody2D>();
        transform.SetPositionAndRotation(new Vector3(transform.position.x, startingY, transform.position.z), transform.rotation);
        jump = autoJump = false; onGround = true;
        spawns.Add(new spawnData(transform.position, Instantiate(spawnPrefab, transform.position, transform.rotation, spawnPool.transform), 0,
            new PowerUpsManager.PowerUpsData()));
        raycastDistance = transform.localScale.y / 2.0f + 0.02f;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
        if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space)) jump = true;
        else if (!autoJump) jump = false;

        if (Input.GetMouseButtonDown(1))
            spawns.Add(new spawnData(transform.position,
                                    Instantiate(spawnPrefab, transform.position, transform.rotation, spawnPool.transform),
                                    crono.getActualTime(),
                                    powerUpsManager.getData() //Info de los powerUps
                                    ));

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
                PlayerDeath();
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            GameManager.instance.ChangeAutoJumpMode();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.instance.ChangeDebugMode();
        }

        if (onGround)
        {
            Unrotate();
            particles.enableEmission = true;
        }
        else
        {
            sprite.Rotate(Vector3.back);
            particles.enableEmission = false;
        }
    }

    private void FixedUpdate()
    {
        if (jump) Jump();
        if (rb.velocity.y < 0) onGround = false;
    }

    void Unrotate()
    {
        Vector3 rotation = sprite.rotation.eulerAngles;
        rotation.z = Mathf.Round(rotation.z / 90) * 90;
        sprite.rotation = Quaternion.Euler(rotation);
    }

    public float getPlayerSpeed() { return speed; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        if (normal == Vector2.down) PlayerDeath();
        else if (normal == Vector2.up) onGround = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        if (normal == Vector2.left) PlayerDeath();
    }


    public void PlayerDeath()
    {
        GameManager.instance.SetDeath(true);
        spawnData lastSpawn = spawns[spawns.Count - 1];
        onGround = false;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        transform.position = lastSpawn.pos;
        GameManager.instance.SetDeathTime(lastSpawn.time);
        crono.setActualTime(lastSpawn.time);

        GetComponent<RestartMusic>().restartMusic((int)(lastSpawn.time * 1000.0));

        //Resetear el estado de los powerUps
        powerUpsManager.ResetData(lastSpawn.powerUpsData);
    }

    private void Jump()
    {
        if (onGround)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            autoJump = jump = onGround = false;
        } 
    }

    internal void AutoJump() { jump = true; autoJump = true; }

    public float getJumpForce() { return jumpForce; }
}