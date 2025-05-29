using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Points")]
    [SerializeField] private float points = 0f;
    [SerializeField] private int starsToGive = 0;
    [Header("Violations")]
    [SerializeField] private int currentTrafficViolations = 0;
    [SerializeField] private int maxTrafficViolations = 3;
    [Header("Game Variables")]
    [SerializeField] private bool hardDifficulty = false;
    [SerializeField] private int easyLevelPedestrianNPCCount = 50;
    [SerializeField] private int easyLevelVehicleNPCCount = 20;
    [SerializeField] private int hardLevelPedestrianNPCCount;
    [SerializeField] private int hardLevelVehicleNPCCount;
    private bool routeSuccessful = false;
    private bool isGameEnded = false;
    [Tooltip("X = minutes, Y = seconds")]
    [SerializeField] private Vector2 gameTime;
    [Header("GameEvents")]
    [SerializeField] private GameEvent sendPointsData;
    [SerializeField] private GameEvent sendTimerData;
    [SerializeField] private GameEvent onEndGame;
    [SerializeField] private GameEvent onTimeScaleChange;
    [SerializeField] private GameEvent onFixedDeltaTimeChange;
    [Header("Next Scene")]
    [Tooltip("Load into Main Menu or enter cutscene name")]
    [SerializeField] private string sceneString;
    [SerializeField] private NPCCount npcs;
    [Header("Violation Settings")]
    [SerializeField] private float slowMoDuration = 2f; 
    [SerializeField] private float slowMoTimeScale = 0.2f; 

    private List<float> violationPointsHolder = new List<float>();
    private float gameTimeInFloat;
    private float targetTime;
    private string failType;
    private bool end = false;
    private float currentHighScore;
    private DataManager dm;
    private void Start()
    {
        npcs.ResetCurrentNPCValues();

        dm = DataManager.Instance;

        float min = gameTime.x * 60f, sec = gameTime.y;
        gameTimeInFloat = min + sec;
        targetTime = gameTimeInFloat;
        currentHighScore = dm.CurrentLevel.HighScore;

        /*if(!hardDifficulty)
        {
            npcs.setMaxPedestrianCount(easyLevelPedestrianNPCCount);
            npcs.setMaxVehicleCount(easyLevelVehicleNPCCount);
            return;
        }
        npcs.setMaxPedestrianCount(hardLevelPedestrianNPCCount);
        npcs.setMaxVehicleCount(hardLevelVehicleNPCCount);*/
    }

    private void Update()
    {
        if (isGameEnded)
        {
            if(end)
                { return; }

            if(!routeSuccessful)
            {
                onEndGame.Raise(this, failType);
                return;
            }
            Debug.Log("Calculating points...");
            calculateStarsToGive(calculateEndOfGamePoints());
            Debug.Log("Game end\nStars Given: "+starsToGive);

            float[] f = new float[3];
            f[0] = points;
            f[1] = Mathf.Clamp((float)starsToGive,0,3);  
            if(points > currentHighScore)
            {
                f[2] = points;
            }
            else
            {
                f[2] = currentHighScore;
            }
            end = true;
            onEndGame.Raise(this,f); //PROBLEMATIC
            
        }

        sendPointsData.Raise(this, points);
        sendTimerData.Raise(this, targetTime);
        checkViolations();
        checkTimeLimit();
    }
    #region Game Functions
    private void endGame()
    {
        isGameEnded = true;
    }

    public void saveGame()
    {
        Levels l = dm.CurrentLevel;
        Levels nl = dm.NextLevel;
        
        l.Stars = starsToGive;
        if(points > l.HighScore)
        {
            l.HighScore = points;
        }

        if(l.Stars >= 2f) //PASSING NUMBER OF STARS TO UNLOCK NEXT LEVEL
        {
            if(!nl.IsUnlocked)
            {
                nl.UnlockLevel();
                dm.saveNext();
            }
        }

        dm.save();
        npcs.ResetCurrentNPCValues();
        SceneLoadManager.Instance.LoadScene(sceneString);
    }

    public void restartGame(Component sender, object data)
    {
        string s = (string)data;

        SceneLoadManager.Instance.LoadScene(s);
    }

    public void exitGame(Component sender, object data)
    {
        SceneLoadManager.Instance.LoadScene((string)data);
    }

    public void endOfRouteReached(Component sender, object data)
    {
        routeSuccessful = true;

        Debug.Log("END OF ROUTE REACHED");

        endGame();
    }

    private float calculateEndOfGamePoints()
    {
        float totalPoints;
        if(routeSuccessful)
        {
            totalPoints = points + 250f + targetTime;
            points = totalPoints;
            return points;
        }
        else
        {
            totalPoints = Mathf.Clamp(points, -1000, 400);
            foreach(float num in violationPointsHolder)
            {
                totalPoints += num;
            }
            points = totalPoints;
            return points;
        }


    }
    #endregion
    #region Violations
    private void checkViolations()
    {
        if(currentTrafficViolations >= maxTrafficViolations)
        {
            failType = "Violations Exceeded";

            endGame();
        }
    }

    public void IncreaseTrafficViolation()
    {
        currentTrafficViolations++;  
        StartCoroutine(SlowDownTimeTemporarily());  
    }
    public void DecreaseTrafficViolation()
    {
        currentTrafficViolations--;
    }

    private IEnumerator SlowDownTimeTemporarily()
    {
        onTimeScaleChange.Raise(this, slowMoTimeScale);
        onFixedDeltaTimeChange.Raise(this, 0.02f * slowMoTimeScale);

        yield return new WaitForSecondsRealtime(slowMoDuration);  

        onTimeScaleChange.Raise(this, 1f);
        onFixedDeltaTimeChange.Raise(this, 0.02f);
        
    }

    #endregion
    #region Points System
    private void calculateStarsToGive(float p)
    {
        if(p >= 750f)
        {
            starsToGive = 3;
        }
        else if(p >= 500f)
        {
            starsToGive = 2;
        }
        else if(p >= 250f)
        {
            starsToGive = 1;
        }
        else
        {
            starsToGive = 0;
        }
    }

    public void addPoints(Component sender, object data)
    {
        if(data.GetType() != typeof(float))
        {
            return;
        }

        float num = (float)data;

        if(num <= 0)
        {
            violationPointsHolder.Add(num);

            IncreaseTrafficViolation();
        }

        points = points + num;
    }
    public void addPoints(int p)
    {
        if (p <= 0)
        {
            violationPointsHolder.Add(p);
        }

        points = points + p;
    }
    void PointsSender()
    {
        sendPointsData.Raise(this, points);
    }
    #endregion
    #region Time Limit
    private void checkTimeLimit()
    {
        if(Timer())
        {
            failType = "Timer Expired";

            endGame();
            return;
        }
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
    #endregion
}
