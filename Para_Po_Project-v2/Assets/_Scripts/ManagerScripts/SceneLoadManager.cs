using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadManager : PersistentSingleton<SceneLoadManager>
{



    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }
}
