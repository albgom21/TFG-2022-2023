using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunc : MonoBehaviour
{
    public void PlayLvl()
    {
        GameManager.instance.setSong(GetComponent<TextMeshProUGUI>().text);
        SceneManager.LoadScene("SampleScene");
    }
}