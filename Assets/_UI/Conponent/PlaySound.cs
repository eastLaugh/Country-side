using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public void Excecute(string soundNames)
    {
        SoundName soundName = (SoundName)Enum.Parse(typeof(SoundName), soundNames);
        //Debug.Log(soundName);
        EventHandler.CallInitSoundEffect(soundName);
    }
}
