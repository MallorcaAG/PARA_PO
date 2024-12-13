using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UISFX : MonoBehaviour
{
    [Tooltip("Ensure that all Audio clips are normalized beforehand to prevent one SFX being louder than another")]
    [SerializeField] private AudioClip[] hoverSFX;
    [Tooltip("Ensure that all Audio clips are normalized beforehand to prevent one SFX being louder than another")]
    [SerializeField] private AudioClip selectSFX, wooshSFX, startgameSFX, backbtnSFX;

    Transform audioListenerPosition;

    private void Start()
    {
        audioListenerPosition = Camera.main.transform;
    }

    public void PlayHoverSFX()
    {
        if(hoverSFX != null || hoverSFX.Length != 0)
        SoundFXManager.Instance.PlayRandomSoundFXClip2D(hoverSFX, audioListenerPosition, 1f);
    }

    public void PlaySelectSFX()
    {
        if(selectSFX != null)
        SoundFXManager.Instance.PlaySoundFXClip2D(selectSFX, audioListenerPosition, 1f);
    }
    public void PlayWooshSFX()
    {
        if(wooshSFX != null)
        SoundFXManager.Instance.PlaySoundFXClip2D(wooshSFX, audioListenerPosition, 1f);
    }

    public void PlayStartSFX()
    {
        if(startgameSFX != null)
        SoundFXManager.Instance.PlaySoundFXClip2D(wooshSFX, audioListenerPosition, 1f);
    }

    public void PlayBackSFX()
    {
        if(backbtnSFX != null)
        SoundFXManager.Instance.PlaySoundFXClip2D(backbtnSFX, audioListenerPosition, 1f);
    }
}
