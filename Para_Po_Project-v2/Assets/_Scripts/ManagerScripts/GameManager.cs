using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private float points = 0f;
    [SerializeField] private int starsToGive = 0;
    [Header("Violations")]
    [SerializeField] private int currentTrafficViolations = 0;
    [SerializeField] private int maxTrafficViolations = 3;
    [Header("Game Variables")]
    [SerializeField] private bool isGameEnded = false;

    private void Update()
    {
        if (isGameEnded)
        {
            calculateStarsToGive();
            Debug.Log("Game end\nStars Given: "+starsToGive);
            return;
        }

        checkViolations();
    }
    #region Game Functions
    private void endGame()
    {
        isGameEnded = true;
    }
    #endregion
    #region Violations
    private void checkViolations()
    {
        if(currentTrafficViolations == maxTrafficViolations)
        {
            endGame();
        }
    }

    public void IncreaseTrafficViolation()
    {
        currentTrafficViolations++;
    }
    #endregion
    #region Points System
    private void calculateStarsToGive()
    {
        if(points >= 750f)
        {
            starsToGive = 3;
        }
        else if(points >= 500f)
        {
            starsToGive = 2;
        }
        else if(points >= 250f)
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
        points = points + (float)data;
    }
    public void addPoints(int p)
    {
        points = points + p;
    }
    #endregion
}
