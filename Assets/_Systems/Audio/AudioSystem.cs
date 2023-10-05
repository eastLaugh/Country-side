using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour
{

    public SoundDetailsList_SO SoundDetailsList_SO;
    public static AudioSystem current;
    [SerializeField] AudioSource musicPlayer;
    public AudioMixer audioMixer;
    private void Awake()
    {
        current = this;
        
    }
    private void Start()
    {
        audioMixer.SetFloat("MasterVolume", Settings.MasterVolume - 10);
        audioMixer.SetFloat("MusicVolume", Settings.MusicVolume - 20);
        audioMixer.SetFloat("EffectVolume", Settings.MasterVolume - 20);
        SetMusic(SoundName.MenuMusic);
        musicPlayer.Play();
    }
    public void SwitchMusic(SoundName soundName)
    {
        SetMusic(soundName);
        musicPlayer.Play();
    }
    void SetMusic(SoundName soundName)
    {
        var soundDetails = SoundDetailsList_SO.GetSoundDetails(soundName);
        musicPlayer.clip = soundDetails.soundClip;
        musicPlayer.volume = soundDetails.soundVolume;
        musicPlayer.pitch = UnityEngine.Random.Range(soundDetails.soundPitchMin, soundDetails.soundPitchMax);
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }
    
}

public enum SoundName
{
    MenuMusic,GameMusic,
    BtnClick,MissionFinish,Costruct,WrongPlace,SlotClick,Demolish
}
