using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Timeline;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform sprite;
    [SerializeField] private GameObject obstacleGenerator;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private GameObject spawnPool;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Crono crono;
    private Rigidbody2D rb;
    private bool jump, onGround;

    private struct spawnData
    {
        public Vector3 pos;
        public GameObject obj;
        public double time;
        
        public spawnData(Vector3 position, GameObject o, double t)
        {
            pos = position;
            obj = o;
            time = t;
        }
    }
    List<spawnData> spawns = new List<spawnData>();

    private RaycastHit2D raycast;
    private float raycastDistance;

    void Start()
    {
        int startingY = (int)(obstacleGenerator.GetComponent<ObstacleGenerator>().getFeatures().GetComponent<ReadTxt>().getScopt()[2] * obstacleGenerator.GetComponent<ObstacleGenerator>().getMultiplierY());
        rb = GetComponent<Rigidbody2D>();
        transform.SetPositionAndRotation(new Vector3(transform.position.x, + startingY, transform.position.z), transform.rotation);
        jump = false; onGround = true;
        spawns.Add(new spawnData(transform.position, Instantiate(spawnPrefab, transform.position, transform.rotation, spawnPool.transform),0));
        raycastDistance = transform.localScale.y / 2.0f + 0.02f;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
        if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space)) jump = true;
        else jump = false;

        if (Input.GetMouseButtonDown(1))           
            spawns.Add(new spawnData(transform.position, 
                                    Instantiate(spawnPrefab, transform.position, transform.rotation, spawnPool.transform),
                                    crono.getActualTime()));
        
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (spawns.Count > 1)
            {
                var obj =  spawns[spawns.Count - 1].obj;
                Destroy(obj);
                spawns.RemoveAt(spawns.Count - 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            for(int i = spawns.Count - 1; i > 0; i--)
            {
                var obj = spawns[i].obj;
                Destroy(obj);
                spawns.RemoveAt(i);
            }
        }

        if (onGround) {
            Unrotate();
            particles.enableEmission = true;

        }
        else {
            sprite.Rotate(Vector3.back);
            particles.enableEmission = false;
        }
    }

    private void FixedUpdate()
    {
        if (onGround && jump)
        {
            //Jump
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * 26.6581f, ForceMode2D.Impulse);
        }

        raycast = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance);
        if (raycast.collider != null) onGround = true;
        else onGround = false;
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
        if (normal == Vector2.down || normal == Vector2.left)
        {
            playerDeath();
        }
    }

    public void playerDeath()
    {
        GameManager.instance.setDeath(true);
        
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        transform.position = spawns[spawns.Count-1].pos;
        GameManager.instance.setDeathTime(spawns[spawns.Count - 1].time);
        crono.setActualTime(spawns[spawns.Count - 1].time);
        
        GetComponent<RestartMusic>().restartMusic((int)(spawns[spawns.Count - 1].time * 1000));
    }
}