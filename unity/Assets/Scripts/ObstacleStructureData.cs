using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStructureData : MonoBehaviour
{
    [SerializeField]
    private float unlevel;
    [SerializeField]
    private float prevX;
    [SerializeField]
    private float postX;
    [SerializeField]
    private int difficulty1To5;
    [SerializeField]
    bool obstacleEnabled;

    public float getUnlevel()
    {
        return unlevel;
    }

    public float getPrevX()
    {
        return prevX;
    }

    public float getPostX()
    {
        return postX;
    }

    public int getDifficulty()
    {
        return difficulty1To5;
    }

    public bool getObstacleEnabled()
    {
        return obstacleEnabled;
    }
}
