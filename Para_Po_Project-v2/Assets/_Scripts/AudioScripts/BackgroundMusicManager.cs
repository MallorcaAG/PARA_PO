using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : PersistentSingleton<BackgroundMusicManager>
{
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip radioTransition;
    [SerializeField] private RadioStations[] radioStations;
    [SerializeField] private AudioSource backgroundMusicObjectPrefab2D;

    private GameObject currentBGM;
    private int i, start;
    private RadioStations currentStation;
    private float targetTime = 3f;

    private void Start()
    {
        PlayMainMenuMusic();
    }

    private void Update()
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        if (Timer())
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                targetTime = 3f;
                StartCoroutine(ChangeRadioStation());
            }
        }
    }

    private void PlayBGM(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if(currentBGM != null)
        {
            Destroy(currentBGM);
        }

        AudioSource audioSource = Instantiate(backgroundMusicObjectPrefab2D, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        currentBGM = audioSource.gameObject;

        Destroy(currentBGM, clipLength);
    }

    public void PlayMainMenuMusic()
    {
        PlayBGM(mainMenuMusic, transform, 1f);
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
            PlayBGM(currentStation.Tracks[j], transform, 1f);

            yield return new WaitForSeconds(currentStation.Tracks[j].length);
        }

        start = 0;

        PlayRadio();
    }

    private IEnumerator ChangeRadioStation()
    {
        StopCoroutine(PlayRadio());

        PlayBGM(radioTransition, transform, 1f);

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
        StopAllCoroutines();

        PlayMainMenuMusic();
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
