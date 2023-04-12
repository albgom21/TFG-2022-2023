using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void goToMenu()
    {
        SceneManager.LoadScene(Constants.NAME_MENU_SCENE);
    }
    public void goToGame()
    {
        SceneManager.LoadScene(Constants.NAME_GAME_SCENE);
    }
}