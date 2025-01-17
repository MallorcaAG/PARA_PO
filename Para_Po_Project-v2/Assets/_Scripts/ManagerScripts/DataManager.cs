using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager : PersistentSingleton<DataManager>
{
    [SerializeField] private Levels currentLevel, nextLevel;

    public Levels CurrentLevel { get { return currentLevel; } set { currentLevel = value; } }
    //public Levels NextLevel { get { return currentLevel; } set { currentLevel = value; } }


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

        if (data.isUnlocked)
        {
            currentLevel.UnlockLevel();
        }
        currentLevel.Stars = data.stars;
        currentLevel.HighScore = data.highScore;
    }

} 
