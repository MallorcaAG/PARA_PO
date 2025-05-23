using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float remainingTime;
    private float saveRemaingingTime;
    private bool running, done = false;

    public Timer(float time)
    {
        remainingTime = time;
        saveRemaingingTime = time;
        done = false;
    }

    private void Update()
    {
        if(running)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0.0f)
            {
                timerEnd();
            }
        }
    }

    public void startTimer()
    {
        running = true;
    }

    public bool timerStatus()
    {
        return done;
    }
    
    public float getRemainingTime()
    {
        return remainingTime;
    }

    public float getTimeLimitSet()
    {
        return saveRemaingingTime;
    }

    private void timerEnd()
    {
        running = false;
        done = true;
    }

    public void resetTimer()
    {
        remainingTime = saveRemaingingTime;
        done = false;
    }

    public void changeTimeLimit(float n)
    {
        saveRemaingingTime = n;
    }

    public void stopTimer()
    {
        running = false;
    }
}
