//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to use
// Programmer Sounds and the Audio Table in your game code. 
//
// Programmer sounds allows the game code to receive a callback at a
// sound-designer specified time and return a sound object to the be
// played within the event.
//
// The audio table is a group of audio files compressed in a Bank that
// are not associated with any event and can be accessed by a string key.
//
// Together these two features allow for an efficient implementation of
// dialogue systems where the sound designer can build a single template 
// event and different dialogue sounds can be played through it at runtime.
//
// This script will play one of three pieces of dialog through an event
// on a key press from the player.
//
// This document assumes familiarity with Unity scripting. See
// https://unity3d.com/learn/tutorials/topics/scripting for resources
// on learning Unity scripting. 
//
// For information on using FMOD example code in your own programs, visit
// https://www.fmod.com/legal
//
//--------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;

[DefaultExecutionOrder(1)]
class SelectMusic : MonoBehaviour
{
    FMOD.Studio.EVENT_CALLBACK callback;

    public FMODUnity.EventReference EventName;

    //[SerializeField]
    private string fileName;

    private FMOD.Studio.EventInstance eventInstance;

    [SerializeField]
    Zone zones;

    //Parametros
    UnderWaterEffect underWaterEffect;
   
#if UNITY_EDITOR
    void Reset()
    {
        EventName = FMODUnity.EventReference.Find("event:/Music");
    }
#endif
    private void Awake()
    {
        fileName = GameManager.instance.getSong() + GameManager.instance.getExtension();
        underWaterEffect = GetComponent<UnderWaterEffect>();
    }

    void Start()
    {
        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        callback = new FMOD.Studio.EVENT_CALLBACK(EventCallback);
        Play();
    }

    void PlayMusic(string key)
    {
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);

        // Pin the key string in memory and pass a pointer through the user data
        GCHandle stringHandle = GCHandle.Alloc(key);
        eventInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

        eventInstance.setCallback(callback);
        eventInstance.start();
        eventInstance.release();
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT EventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr stringPtr;
        instance.getUserData(out stringPtr);

        // Get the string object
        GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
        String key = stringHandle.Target as String;

        switch (type)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));

                    if (key.Contains("."))
                    {
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    else
                    {
                        FMOD.Studio.SOUND_INFO dialogueSoundInfo;
                        var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                        if (keyResult != FMOD.RESULT.OK)
                        {
                            break;
                        }
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    break;
                }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new FMOD.Sound(parameter.sound);
                    sound.release();

                    break;
                }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                    stringHandle.Free();

                    break;
                }
        }
        return FMOD.RESULT.OK;
    }

    public void Play()
    {
        PlayMusic(fileName);
        if (underWaterEffect) underWaterEffect.setEventInstance(eventInstance);
        if (zones) zones.setEventInstance(eventInstance);
    }
    public void playTime(int t)
    {
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);

        // Pin the key string in memory and pass a pointer through the user data
        GCHandle stringHandle = GCHandle.Alloc(fileName);
        eventInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

        eventInstance.setCallback(callback);
        eventInstance.setTimelinePosition(t);
        eventInstance.start();
        eventInstance.release();

        if (underWaterEffect) underWaterEffect.setEventInstance(eventInstance);
        if (zones) zones.setEventInstance(eventInstance);
    }

    public void Stop()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public FMOD.Studio.EventInstance getEventInstance()
    {
        return eventInstance;
    }
}