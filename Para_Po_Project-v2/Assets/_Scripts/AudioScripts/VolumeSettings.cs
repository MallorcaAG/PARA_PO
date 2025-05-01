using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private void OnEnable()
    {
        Initialize();
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    private void Initialize()
    {
        if(DataManager.Instance == null)
        {
            masterSlider.value = 0.5f;
            musicSlider.value = 0.5f;
            sfxSlider.value = 0.5f;
            return;
        }

        masterSlider.value = DataManager.Instance.MasterVolumeSliderValue;
        musicSlider.value = DataManager.Instance.MusicVolumeSliderValue;
        sfxSlider.value = DataManager.Instance.SFXVolumeSliderValue;
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        myMixer.SetFloat("masterVolume", Mathf.Log10(volume)*20);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        myMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
    }

    public void SendDataMasterVolume(float level)
    {
        DataManager.Instance.MasterVolumeSliderValue = level;
    }

    public void SendDataSFXVolume(float level)
    {
        DataManager.Instance.MasterVolumeSliderValue = level;
    }

    public void SendDataMusicVolume(float level)
    {
        DataManager.Instance.MasterVolumeSliderValue = level;
    }
}
