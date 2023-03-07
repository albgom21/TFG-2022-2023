using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice2: MonoBehaviour {
    private AudioSource head;
    private AudioSource tail;
    //public AudioClip sound01, sound02;
    public AudioClip[] pcmDataHeads, pcmDataTails;
    private int nHeads, nTails;

    public float overlapTime=0.01f;
    private int overlapSamples;
    private int sRATE;

    void Awake(){
        sRATE = AudioSettings.outputSampleRate;
        nHeads = pcmDataHeads.Length;
        nTails = pcmDataTails.Length;
        head = gameObject.AddComponent<AudioSource>();        
        tail = gameObject.AddComponent<AudioSource>();        

        //  comprobar que son mono
        for (int i=0; i<nHeads; i++) {
          if (pcmDataHeads[i].channels!=1) throw new System.Exception("Tienen que ser muestras mono!!");
        }
        for (int i=0; i<nTails; i++) {
          if (pcmDataTails[i].channels!=1) throw new System.Exception("Tienen que ser muestras mono!!");
        }

        overlapSamples = (int) Mathf.Round(sRATE*overlapTime);
    }

    void Start(){

    }


    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            int h = Random.Range(0, nHeads), t = Random.Range(0, nTails);
            head.clip = pcmDataHeads[h];
            tail.clip = pcmDataTails[t];

            // los fades se aplican sobre el clip original, el pitch se aplica luego. 
            // No hay que tenerlo en cuenta para aplicar los fades
            FadeOut(head.clip);
            FadeIn(tail.clip);
            

            double clipLength = ((head.clip.samples-overlapSamples) / head.pitch);        
            Debug.Log($"head {h} p tail {t}  sRATE: {sRATE}     lengthHead {clipLength}   overlapSamples {overlapSamples}");

            head.Play();
            tail.PlayScheduled(AudioSettings.dspTime+clipLength/sRATE);
        }
    }

    //  crossFade out en [0,overlapTime]   sqrt((overlapTime-t)/overlapTime)  
    void FadeOut(AudioClip clip){
        // pasamos clip a un array de samples
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);

        // ini: sample inicial donde aplicar fadeOut
        int ini=samples.Length-overlapSamples;
        for (int i=0; i<overlapSamples; i++){
            // mutiplicamos sample por valor de fadeOut. 
            // hay que convertir sample i a tiempo t = i/SRATE (regla de 3)
            samples[ini+i] *= Mathf.Sqrt((overlapTime-(i/sRATE))/overlapTime);
        }

        // volcamos array al clip
        clip.SetData(samples, 0);    
    }


    //  crossFadeIn en [0,overlapTime]   sqrt(t/overlapTime)  
    void FadeIn(AudioClip clip){
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);
        
        for (int i=0; i<overlapSamples; i++){
            // ojo con la conversión de sample a tiempo
            samples[i] *= Mathf.Sqrt((i/sRATE)/overlapTime);
        }
        clip.SetData(samples, 0);    
    }




}
