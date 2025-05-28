using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class DataManager : PersistentSingleton<DataManager>
{
    [SerializeField] private Levels currentLevel, nextLevel;
    [SerializeField] private float masterVolumeSliderValue = 0.5f, sfxVolumeSliderValue = 0.5f, musicVolumeSliderValue = 0.5f;

    public Levels CurrentLevel { get { return currentLevel; } set { currentLevel = value; } }
    //public Levels NextLevel { get { return currentLevel; } set { currentLevel = value; } }
    public float MasterVolumeSliderValue { get { return masterVolumeSliderValue; } set { masterVolumeSliderValue = value; } }
    public float SFXVolumeSliderValue { get { return sfxVolumeSliderValue; } set { sfxVolumeSliderValue = value; } }
    public float MusicVolumeSliderValue { get {return musicVolumeSliderValue;} set { musicVolumeSliderValue = value; } }    

    public void save()
    {
        SaveSystem.SaveLevel(currentLevel);
    }

    public void load()
    {
        SaveData data = SaveSystem.LoadLevel(currentLevel);

        if (data == null)
        {
            return;
        }

        Debug.Log(data.isUnlocked);
        Debug.Log(data.stars);
        Debug.Log(data.highScore);

        if (data.isUnlocked)
        {
            currentLevel.UnlockLevel();
        }
        currentLevel.Stars = data.stars;
        currentLevel.HighScore = data.highScore;
    }

} 
