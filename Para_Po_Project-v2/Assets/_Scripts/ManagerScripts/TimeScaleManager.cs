using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : Singleton<TimeScaleManager>
{

    public void onTimeScaleChange(Component sender, object data)
    {
        Time.timeScale = (float)data;
    }

}
