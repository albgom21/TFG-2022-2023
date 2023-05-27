using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void goToMenu()
    {
        // Sonido clic
        FMODUnity.RuntimeManager.PlayOneShot("event:/MenuSelection");

        // Reestablecer timeScale
        Time.timeScale = 1f;

        SceneManager.LoadScene(Constants.NAME_MENU_SCENE);
    }

    public void goToGame()
    {
        // Sonido clic
        FMODUnity.RuntimeManager.PlayOneShot("event:/MenuSelection");

        // Reestablecer timeScale
        Time.timeScale = 1f;

        SceneManager.LoadScene(Constants.NAME_GAME_SCENE);
    }
}