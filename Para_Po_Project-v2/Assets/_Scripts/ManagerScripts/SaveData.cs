using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool isUnlocked;
    public int stars;
    public float highScore;

    public SaveData(Levels level)
    {
        isUnlocked = level.IsUnlocked;
        stars = level.Stars;
        highScore = level.HighScore;
    }
    
}
