using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GravesAgudos : MonoBehaviour
{
    public Image img_grave;
    public Image img_agudo;
    public ReadTxt input;

    private float[,] graves;
    private float[,] agudos;

    int rows = 0;
    int cols = 0;

    void Start()
    {
        graves = input.getGraves();
        agudos = input.getAgudos();

        for (int i = 0; i < graves.GetLength(0); i++)
        {
            for (int j = 0; j < graves.GetLength(1); j++)
            {
                if (j == 0)
                {
                    Debug.Log("TIEMPO: " + graves[i, j]);
                    Invoke("ChangeSize", graves[i, j]);
                }
            }
        }

    }
    
    private void ChangeSize()
    {
        img_grave.transform.localScale = new Vector3(graves[rows, cols], graves[rows, cols], img_grave.transform.localScale.z);
        if (cols + 1 >= graves.GetLength(1))
        {
            rows++;
            cols = 0;
        }
        else
            cols++;
        Debug.Log("x: " + graves[rows, cols]);

    }

    private void Update()
    {

    }
}