using System.Collections;
using System.Collections.Generic;
using log4net.Core;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void setMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }


    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
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
