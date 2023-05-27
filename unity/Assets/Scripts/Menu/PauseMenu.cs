using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private float timeScale;
    private ControlMusic music;


    [SerializeField]
    private GameObject pauseMenu;

    private void Awake()
    {
        pauseMenu.active = false;
        music = GameObject.FindGameObjectWithTag("Player")?.GetComponent<ControlMusic>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                resumeGame();
            else
                pauseGame();
        }
    }

    void pauseGame()
    {
        timeScale = Time.timeScale;  // Guardar timeScale anterior
        Time.timeScale = 0f;         // Pausar el tiempo
                                     
        isPaused = true;             // Estado de pausa
                                     
        music?.StopMusic();          // Parar la musica
                                     
        pauseMenu.active = true;     // Mostrar menu de pausa
    }

    public void resumeGame()
    {
        Time.timeScale = timeScale;  // Reanudar el tiempo

        isPaused = false;            // Estado de pausa

        music?.ResumeMusic();        // Reactivar la musica

        pauseMenu.active = false;    // Quitar menu de pausa

    }
    public void closeApp()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}