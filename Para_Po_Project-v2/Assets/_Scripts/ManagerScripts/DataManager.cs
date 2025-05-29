using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class DataManager : PersistentSingleton<DataManager>
{
    [SerializeField] private Levels currentLevel, nextLevel;
    [SerializeField] private float masterVolumeSliderValue = 0.5f, sfxVolumeSliderValue = 0.5f, musicVolumeSliderValue = 0.5f;

    private int currentIterator;

    public Levels CurrentLevel { get { return currentLevel; } set { currentLevel = value; } }
    public Levels NextLevel { get { return nextLevel; } set { nextLevel = value; } }
    public float MasterVolumeSliderValue { get { return masterVolumeSliderValue; } set { masterVolumeSliderValue = value; } }
    public float SFXVolumeSliderValue { get { return sfxVolumeSliderValue; } set { sfxVolumeSliderValue = value; } }
    public float MusicVolumeSliderValue { get {return musicVolumeSliderValue;} set { musicVolumeSliderValue = value; } }    
    public int CurrentIterator { get { return currentIterator; } set { currentIterator = value; } }

    public void save()
    {
        Debug.Log("Saving Game");

        SaveSystem.SaveLevel(currentLevel);
    }

    public void saveNext()
    {
        Debug.Log("Saving Game");

        SaveSystem.SaveLevel(NextLevel);
    }

    public void load()
    {
        SaveData data = SaveSystem.LoadLevel(currentLevel);

        if (data == null)
        {
            return;
        }

        Debug.Log(currentLevel.name + " is unlocked:" + data.isUnlocked);
        Debug.Log(currentLevel.name + " stars:" + data.stars);
        Debug.Log(currentLevel.name + " high score:" + data.highScore);

        if (data.isUnlocked)
        {
            currentLevel.UnlockLevel();
        }
        currentLevel.Stars = data.stars;
        currentLevel.HighScore = data.highScore;
    }

    public void delete()
    {
        SaveSystem.DeleteSaveData(currentLevel);
    }

} 
