using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void goToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Constants.NAME_MENU_SCENE);
    }
    public void goToGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Constants.NAME_GAME_SCENE);
    }
}