using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBehaviour : MonoBehaviour
{
    private GameObject menu;
    private RestartMusic music;
    RectTransform[] botones;
    private void Start()
    {
        menu = GameObject.FindGameObjectWithTag("EndMenu");
        music = GameObject.FindGameObjectWithTag("Player").GetComponent<RestartMusic>();

        // Obtener los hijos del objeto padre
        botones = menu.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform b in botones)
            b.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (RectTransform b in botones)
            b.gameObject.SetActive(true);
        music.StopMusic();
        Time.timeScale = 0f;
        GameManager.instance.SetEnd(true);
    }
}