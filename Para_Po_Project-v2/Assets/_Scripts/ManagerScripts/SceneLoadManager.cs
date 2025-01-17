using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : PersistentSingleton<SceneLoadManager>
{
    [SerializeField] private Levels[] Levels;

    private DataManager data;

    private void Start()
    {
        data = DataManager.Instance;
        //CheckLevelsArray();
        /*if(data.CurrentLevel == null)
        {
            
        }*/

        LoadLevelsData();
    }

    #region SceneManagement
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }

    public void LoadLevelsScenes(Levels lvl)
    {
        SceneManager.LoadScene(lvl.Scenes[0], LoadSceneMode.Single);

        for (int i = 1; i < lvl.Scenes.Length; i++)
        {
            SceneManager.LoadScene(lvl.Scenes[i], LoadSceneMode.Additive);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    #endregion

    #region DataLoadingHandling
    private void LoadLevelsData()
    {
        for (int i = 0; i < Levels.Length; i++)
        {
            data.CurrentLevel = Levels[i];
            data.load();
        }

        data.CurrentLevel = null;
    }

    /*private void CheckLevelsArray()
    {
        if (Levels.Length == LevelsManager.Instance.Levels.Count)
        {
            return;
        }

        Debug.LogWarning("Array size mismatch\nSceneLoadManager's levels: " + Levels.Length + "\nLevelsManager's levels: " + LevelsManager.Instance.Levels.Count);

    }*/
    #endregion
}
