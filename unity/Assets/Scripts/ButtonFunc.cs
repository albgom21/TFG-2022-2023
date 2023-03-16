using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunc : MonoBehaviour
{
    public string extension;
    public void PlayLvl()
    {
        GameManager.instance.setSong(GetComponent<TextMeshProUGUI>().text);
        GameManager.instance.setExtension(extension);
        SceneManager.LoadScene("SampleScene");
    }
    public void setExtension(string s)
    {
        extension = s;
    }
}