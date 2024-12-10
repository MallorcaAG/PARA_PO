using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum VehicleType
{
    TRICYCLE, JEEPNEY, BUS
}

[CreateAssetMenu(fileName = "Level 0", menuName = "Level Data")]
public class Levels : ScriptableObject
{
    [SerializeField] private bool isUnlocked = false;
    [SerializeField] private VehicleType vehicleType;
    [SerializeField] private string routeName;
    [SerializeField] private Sprite[] routeMap;
    [SerializeField] private Sprite[] routeLandmarkImages;
    [SerializeField] private LevelSceneHolder scenes;
    [SerializeField] private int stars = 0;
    [SerializeField] private float highScore = 0;
    

    #region Getter/Setter Functions
    public void UnlockLevel()
    {
        isUnlocked = true;
    }
    public bool IsUnlocked { get { return isUnlocked; } }
    public VehicleType VehicleType { get { return vehicleType; } }
    public string RouteName { get { return routeName; } }
    public Sprite getRouteMap() 
    {
        return routeMap[0];
    }
    public Sprite getRouteLine()
    {
        return routeMap[1];
    }
    public Sprite[] RouteLandmarkImages { get { return routeLandmarkImages; } }
    public Scene[] Scenes { get { return scenes.levelScenes; } }
    public int Stars { get { return stars; } set { stars = value; } }
    public float HighScore { get { return highScore; } set { highScore = value; } }  
    #endregion
}

