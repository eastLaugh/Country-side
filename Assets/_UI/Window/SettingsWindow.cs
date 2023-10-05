using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] Button BtnClose;
    [SerializeField] Slider MusicVolumeSlider;
    [SerializeField] Slider EffectVolumeSlider;
    [SerializeField] TimeController timeController;
    [SerializeField] GameObject window;
    [SerializeField] Toggle gridOn;
    [SerializeField] GameObject Grid;
    [SerializeField] AudioMixer audioMixer;
    void Start()
    {
        gridOn.isOn = Settings.GridOn;
        MusicVolumeSlider.value = Settings.MusicVolume;
        EffectVolumeSlider.value = Settings.EffectVolume;
        BtnClose.onClick.AddListener(()=>{
            timeController.Continue();
            window.SetActive(false);
        });
        gridOn.onValueChanged.AddListener((value) =>
        {
            Settings.GridOn = value;
            Grid.SetActive(value);
        });
        MusicVolumeSlider.onValueChanged.AddListener((float newValue) => {
            Settings.MusicVolume = newValue;
            audioMixer.SetFloat("MusicVolume", newValue * 2 - 80);
        });
        EffectVolumeSlider.onValueChanged.AddListener((float newValue) => {
            Settings.EffectVolume = newValue;
            audioMixer.SetFloat("EffectVolume", newValue * 2 - 80);
        });
    }
    public void Refresh()
    {
        gridOn.isOn = Settings.GridOn;
        MusicVolumeSlider.value = Settings.MusicVolume;
        EffectVolumeSlider.value = Settings.EffectVolume;
    }
    
    private void OnDisable()
    {
        BtnClose.onClick.RemoveAllListeners();
        gridOn.onValueChanged.RemoveAllListeners();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
