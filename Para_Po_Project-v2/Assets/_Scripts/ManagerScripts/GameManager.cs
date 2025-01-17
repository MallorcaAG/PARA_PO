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
    [SerializeField] private bool routeSuccessful = false;
    [SerializeField] private bool isGameEnded = false;
    [Tooltip("X = minutes, Y = seconds")]
    [SerializeField] private Vector2 gameTime;
    [Header("GameEvents")]
    [SerializeField] private GameEvent sendPointsData;
    [SerializeField] private GameEvent sendTimerData;
    [SerializeField] private GameEvent onEndGame;
    [Header("Next Scene")]
    [Tooltip("Load into Main Menu or enter cutscene name")]
    [SerializeField] private string sceneString;


    private List<float> violationPointsHolder = new List<float>();
    private float gameTimeInFloat;
    private float targetTime;
    private string failType;
    private bool end = false;
    private float currentHighScore;

    private void Start()
    {
        float min = gameTime.x * 60f, sec = gameTime.y;
        gameTimeInFloat = min + sec;
        targetTime = gameTimeInFloat;
        currentHighScore = DataManager.Instance.CurrentLevel.HighScore;
        StartCoroutine(PointsSender());
    }

    private void Update()
    {
        if (isGameEnded)
        {
            if(end)
                return;

            if(!routeSuccessful)
            {
                onEndGame.Raise(this, failType);
                return;
            }

            calculateStarsToGive(calculateEndOfGamePoints());
            Debug.Log("Game end\nStars Given: "+starsToGive);

            float[] f = new float[3];
            f[0] = points;
            f[1] = (float)starsToGive;
            if(points > currentHighScore)
            {
                f[2] = points;
            }
            else
            {
                f[2] = currentHighScore;
            }
            onEndGame.Raise(this,f);
            end = true;
            return;
        }

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
        DataManager.Instance.save();
        SceneLoadManager.Instance.LoadScene(sceneString);
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

            return totalPoints;
        }
        else
        {
            totalPoints = Mathf.Clamp(points, -1000, 400);
            foreach(float num in violationPointsHolder)
            {
                totalPoints += num;
            }

            return totalPoints;
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
    }
    public void DecreaseTrafficViolation()
    {
        currentTrafficViolations--;
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
    IEnumerator PointsSender()
    {
        yield return new WaitForSeconds(1f);

        sendPointsData.Raise(this, points);

        if (!isGameEnded)
        {
            yield break;
        }

        PointsSender();
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
