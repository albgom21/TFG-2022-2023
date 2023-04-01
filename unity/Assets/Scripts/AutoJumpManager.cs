using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoJumpManager : MonoBehaviour
{
    [SerializeField] private bool autoJumpEnabled;
    private List<GameObject> autoJumpInstances;
    private Color autoJumpOnColor, autoJumpOffColor;
    // Start is called before the first frame update
    void Start()
    {
        autoJumpInstances = new List<GameObject>();
        autoJumpOnColor = Color.green;
        autoJumpOffColor = Color.red;
        //updateInstances();
    }

    public bool getAutoJumpEnabled() { return autoJumpEnabled; }
    public void changeAutoJumpMode()
    {
        autoJumpEnabled = !autoJumpEnabled;
        updateInstances();
    }

    public void updateInstances()
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

    public void addInstance(GameObject newAutoJump)
    {
        autoJumpInstances.Add(newAutoJump);
    }
}
