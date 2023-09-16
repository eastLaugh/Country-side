using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider EffectVolumeSlider;
    public AudioMixer audioMixer;
    private void OnEnable()
    {
        MasterVolumeSlider.value = Settings.MasterVolume;
        MusicVolumeSlider.value = Settings.MusicVolume;
        EffectVolumeSlider.value = Settings.EffectVolume;
       MasterVolumeSlider.onValueChanged.AddListener((float newValue)=>{
            Settings.MasterVolume = newValue;
            audioMixer.SetFloat("MasterVolume", newValue-10);
        }) ;
        MusicVolumeSlider.onValueChanged.AddListener((float newValue) => {
            Settings.MusicVolume = newValue;
            audioMixer.SetFloat("MusicVolume", newValue-20);
        });
        EffectVolumeSlider.onValueChanged.AddListener((float newValue) => {
            Settings.EffectVolume = newValue;
            audioMixer.SetFloat("EffectVolume", newValue - 20);
        });
    }
    private void OnDisable()
    {
        MasterVolumeSlider.onValueChanged.RemoveAllListeners();
        MusicVolumeSlider.onValueChanged.RemoveAllListeners();
        EffectVolumeSlider.onValueChanged.RemoveAllListeners();
    }
}
