using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Vector2 movSpeed;
    private Vector2 offset;
    private Material mat;
    void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        offset = movSpeed * Time.deltaTime;
        mat.mainTextureOffset += offset;
    }
}