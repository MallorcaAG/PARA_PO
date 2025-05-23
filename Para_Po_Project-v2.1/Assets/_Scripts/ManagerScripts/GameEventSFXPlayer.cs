using System.Collections;
using UnityEngine;

public class GameEventSFXPlayer : MonoBehaviour
{
    private Transform playerPos;
    private Transform audioListenerPosition;
    private bool initialized = false;

    private void OnEnable()
    {
        StartCoroutine(InitializeDelayed());
    }

    private IEnumerator InitializeDelayed()
    {
        yield return null; 
        InitializeReferences();
    }

    public void InitializeReferences()
    {
        if (!initialized)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerPos = playerObj.transform;
                initialized = true;
            }
            else
            {
                Debug.LogWarning("GameEventSFXPlayer: Player object not found. Ensure it has the 'Player' tag and is active.");
                initialized = false;

            }

            if (Camera.main != null)
            {
                audioListenerPosition = Camera.main.transform;
                initialized = true;
            }
            else
            {
                Debug.LogWarning("GameEventSFXPlayer: Main camera not found. Assign the 'MainCamera' tag to your camera.");
                initialized = false;
            }
        }
    }

    public void deInitialize()
    {
        initialized = false;
    }

    public void PlaySFXviaGameEvent(Component sender, object data)
    {
        if (!initialized)
        {
            InitializeReferences(); 
        }

        AudioClip clip = data as AudioClip;
        if (clip == null)
        {
            Debug.LogError("GameEventSFXPlayer: Provided data is not a valid AudioClip.");
            return;
        }

        if (playerPos != null)
        {
            SoundFXManager.Instance.PlaySoundFXClip(clip, playerPos, 1f);
        }
        else if (audioListenerPosition != null)
        {
            SoundFXManager.Instance.PlaySoundFXClip(clip, audioListenerPosition, 1f);
        }
        else
        {
            Debug.LogWarning("GameEventSFXPlayer: No valid position found to play the sound effect.");
        }
    }
}
