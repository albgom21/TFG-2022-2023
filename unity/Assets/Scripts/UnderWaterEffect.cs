using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterEffect : MonoBehaviour
{

    FMOD.Studio.EventInstance eventInstance;
    float underWaterParameter;

    [SerializeField]
    float alturaComienzoAgua;

    [SerializeField]
    float alturaAguaMaxima;

    float regionAgua;

    // Start is called before the first frame update
    void Start()
    {
        underWaterParameter = 0.0f;
        regionAgua = alturaComienzoAgua - alturaAguaMaxima;
    }

    // Update is called once per frame
    void Update()
    {
        if (underWaterParameter != 1.0f && transform.position.y < alturaAguaMaxima)
        {
            //Al fondo del agua, el parámetro es 1.0f
            underWaterParameter = 1.0f;
            eventInstance.setParameterByName("Underwater", underWaterParameter);
        }

        else if (underWaterParameter != 0.0f && transform.position.y > alturaComienzoAgua)
        {
            //Fuera del agua, el parámetro es 0.0f
            underWaterParameter = 0.0f;
            eventInstance.setParameterByName("Underwater", underWaterParameter);
        }

        else if (transform.position.y > alturaAguaMaxima && transform.position.y < alturaComienzoAgua)
        {
            //Cuando estás dentro del agua, el parámetro se encontrará entre 0.5 y 1.0

            //Ejemplo: Agua Máxima es 1, Comienzo Agua es 11, transform.y es 5, por lo que region tiene que dar 0,4 y 
            //underWaterParameter 0.7

            float region = (transform.position.y - alturaAguaMaxima) / regionAgua;  //Valor entre 0.0 y 1.0
            underWaterParameter = (region / 2.0f) + 0.5f;                           //Valor entre 0.5 y 1.0

            eventInstance.setParameterByName("Underwater", underWaterParameter);
        }

    }
    
    public void setEventInstance(FMOD.Studio.EventInstance eI)
    {
        eventInstance = eI;
    }
}
