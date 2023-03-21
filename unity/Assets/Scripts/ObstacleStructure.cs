using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStructure : MonoBehaviour
{
    [SerializeField]
    private float unlevel;
    [SerializeField]
    private float prevX;
    [SerializeField]
    private float postX;
  

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
}
