using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GravesAgudos : MonoBehaviour
{
    public Image img_grave;
    public Image img_agudo;
    public ReadTxt input;

    private List<float> agudosTiempo = new List<float>();
    private List<float> gravesTiempo = new List<float>();
    private List<float> agudosValoresNorm = new List<float>();
    private List<float> gravesValoresNorm = new List<float>();

    int contA = 0;
    int contG = 0;

    void Start()
    {
        gravesTiempo = input.GetGravesTiempo();
        gravesValoresNorm = input.GetGravesValoresNorm();

        foreach (float time in gravesTiempo)
            Invoke("ChangeSizeGrave", time);

        agudosTiempo = input.GetAgudosTiempo();
        agudosValoresNorm = input.GetAgudosValoresNorm();

        foreach (float time in agudosTiempo)
            Invoke("ChangeSizeAgudo", time);
    }
    
    private void ChangeSizeAgudo()
    {
        img_agudo.transform.localScale = new Vector3(agudosValoresNorm[contA], agudosValoresNorm[contA], img_agudo.transform.localScale.z);
        //Debug.Log("x: " + agudosValoresNorm[contA]);
        contA++;
    }

    private void ChangeSizeGrave()
    {
        img_grave.transform.localScale = new Vector3(gravesValoresNorm[contG], gravesValoresNorm[contG], img_grave.transform.localScale.z);
        contG++;
    }

    private void Update()
    {
    }
}