using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadScene1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadScene2()
    {
        SceneManager.LoadScene("Level2");
    }
}
