using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : PersistentSingleton<SoundFXManager>
{
    [SerializeField] private AudioSource soundFXObjectPrefab;
    [SerializeField] private AudioSource soundFXObjectPrefab2D;


    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObjectPrefab, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObjectPrefab, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlaySoundFXClip2D(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObjectPrefab2D, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip2D(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObjectPrefab2D, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

}
