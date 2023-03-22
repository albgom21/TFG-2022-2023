using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void goToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void goToGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
