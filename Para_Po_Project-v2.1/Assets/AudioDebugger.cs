using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDebugger : MonoBehaviour
{
    void Start()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (source.isPlaying)
            {
                Debug.Log($"AudioSource playing: {source.clip.name} on GameObject {source.gameObject.name}");
            }
        }
    }
}
