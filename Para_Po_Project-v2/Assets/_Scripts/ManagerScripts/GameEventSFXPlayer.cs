using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSFXPlayer : MonoBehaviour
{
    private Transform playerPos;
    private Transform audioListenerPosition;


    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        audioListenerPosition = Camera.main.transform;
    }

    public void PlaySFXviaGameEvent(Component sender, object data)
    {
        AudioClip clip = (AudioClip)data;

        if(playerPos != null)
        {
            SoundFXManager.Instance.PlaySoundFXClip(clip, playerPos, 1f);
        }
        else
        {
            SoundFXManager.Instance.PlaySoundFXClip(clip, audioListenerPosition, 1f);
        }
    }
}
