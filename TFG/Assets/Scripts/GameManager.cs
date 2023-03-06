using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance_;
    private Vector3 startPosition;

    void Awake()     //  Comprobar que solo hay un GameManager.
    {
        if (instance_ == null)
        {
            instance_ = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);
    }

    private void Start()
    {
        startPosition = new Vector3();
    }

    public void setStartPosition(Vector3 pos)
    {
        startPosition = pos;
    }

    public Vector3 getStartPosition()
    {
        return startPosition;
    }
}
