using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AutoJumpManager : MonoBehaviour
{
    [SerializeField] private bool autoJumpEnabled;
    private List<GameObject> autoJumpInstances;
    private Color autoJumpOnColor, autoJumpOffColor;
    private PlayerMovement player;
    [SerializeField] private float timeToCreateRespawn;
    private float timeCounter;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (GameManager.instance != null)
            GameManager.instance.SetAutoJumpManager(this);
        autoJumpInstances = new List<GameObject>();
        autoJumpOnColor = Color.green;
        autoJumpOffColor = Color.red;
        timeCounter = 0.0f;
    }

    private void Update()
    {
        if (autoJumpEnabled)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= timeToCreateRespawn)
            {
                player.CreateRespawn();
                timeCounter = 0.0f;
            }
        }
    }

    public bool GetAutoJumpEnabled() { return autoJumpEnabled; }
    public void ChangeAutoJumpMode()
    {
        autoJumpEnabled = !autoJumpEnabled;
        UpdateInstances();
    }

    public void UpdateInstances()
    {
        Color newColor;
        if (autoJumpEnabled) newColor = autoJumpOnColor;
        else newColor = autoJumpOffColor;
        for (int i = 0; i < autoJumpInstances.Count; ++i)
        {
            autoJumpInstances[i].GetComponent<AutoJump>().enabled = autoJumpEnabled;
            autoJumpInstances[i].GetComponent<CircleCollider2D>().enabled = autoJumpEnabled;
            autoJumpInstances[i].GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    public void AddInstance(GameObject newAutoJump)
    {
        //if (player != null) newAutoJump.GetComponent<AutoJump>().SetPlayerVariables(player);
        //else Debug.LogError("No player attached to the AutoJump Manager");       
        autoJumpInstances.Add(newAutoJump);
    }
}
