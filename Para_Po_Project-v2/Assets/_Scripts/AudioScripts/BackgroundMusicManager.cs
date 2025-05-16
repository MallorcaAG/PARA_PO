using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : PersistentSingleton<BackgroundMusicManager>
{
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip radioTransition;
    [SerializeField] private RadioStations[] radioStations;
    [SerializeField] private AudioSource audioSource;
    [SerializeField][Range(0f,1f)] private float defaultVolume = 1f;


    private int i, start;
    private RadioStations currentStation;
    private float targetTime = 3f;
    private bool allowPlayerInput = false;

    private void Start()
    {
        StartCoroutine(PlayMainMenuMusic());
    }

    private void Update()
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        if (Timer())
        {
            if (Input.GetKeyDown(KeyCode.R) && allowPlayerInput)
            {
                InitRadio();
            }
        }
    }

    public void AllowPlayerInput()
    {
        allowPlayerInput = true;
    }

    public void DisallowPlayerInput()
    {
        allowPlayerInput = false;
    }
    public void InitRadio()
    {
        StopAllCoroutines();

        //Debug.Log("Initializing Radio");

        targetTime = 3f;
        StartCoroutine(ChangeRadioStation());
    }

    private void PlayBGM(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();
    }

    public IEnumerator PlayMainMenuMusic()
    {
        //Debug.Log("Playing Main Menu music");

        PlayBGM(mainMenuMusic, transform, defaultVolume);

        yield return new WaitForSeconds(mainMenuMusic.length);

        ReplayMainMenuMusic();
    }

    private void ReplayMainMenuMusic()
    {
        StartCoroutine(PlayMainMenuMusic());
    }

    public void StartRadio()
    { 
        i = 0;
        currentStation = radioStations[i];
        start = Random.Range(0, currentStation.Tracks.Length);
        StartCoroutine(PlayRadio());
    }

    private IEnumerator PlayRadio()
    {
        for(int j = start; j < currentStation.Tracks.Length; j++)
        {
            //Debug.Log("NOW PLAYING: " + currentStation.Tracks[j].name);

            PlayBGM(currentStation.Tracks[j], transform, defaultVolume);

            yield return new WaitForSeconds(currentStation.Tracks[j].length);
        }

        start = 0;

        PlayRadio();
    }

    private IEnumerator ChangeRadioStation()
    {
        StopCoroutine(PlayRadio());

        PlayBGM(radioTransition, transform, defaultVolume);

        yield return new WaitForSeconds(radioTransition.length);

        if (i < radioStations.Length)
        {
            i++;
            currentStation = radioStations[i];
            start = Random.Range(0, currentStation.Tracks.Length);
            StartCoroutine(PlayRadio());
        }
        else
        {
            StartRadio();
        }
    }

    public void StopRadio()
    {
        //Debug.Log("Stopping Radio");

        StopAllCoroutines();

        StartCoroutine(PlayMainMenuMusic());
    }

    public void StopAllMusic()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }

    private bool Timer()
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            return true;
        }

        return false;
    }
}
