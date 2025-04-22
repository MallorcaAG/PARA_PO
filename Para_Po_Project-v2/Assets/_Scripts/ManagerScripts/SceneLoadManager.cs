using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : PersistentSingleton<SceneLoadManager>
{
    [Header("Variables")]
    [SerializeField] private string[] tips;
    [SerializeField] private bool loadIntoLevelSelectionPanel = false;
    private int levelInUnlockedLevels;
    [Header("References")]
    [Tooltip("ENSURE WHAT IS ASSIGNED BELOW IS THE SAME AS WHAT IS ASSIGNED IN THE LEVELSMANGER \n" +
             "(No clue what how to link these two variables together) (No clue what'll happen if not the same but I dont wanna find out)")]
    [SerializeField] private Levels[] Levels;
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI tipTextBox;

    private float target;

    private DataManager data;

    private void Start()
    {
        data = DataManager.Instance;

        LoadLevelsData();
    }

    private void Update()
    {
        progressBar.value = Mathf.MoveTowards(progressBar.value, target, 5f * Time.unscaledDeltaTime);
    }

    #region SceneManagement
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }

    public async void LoadLevelsScenes(Levels lvl, int unlockedLvlPos)  //GOD I HOPE THIS WORKS
    {
        target = 0;
        progressBar.value = 0;
        levelInUnlockedLevels = unlockedLvlPos;

        int scenesSize = lvl.Scenes.Length;

        var scene = SceneManager.LoadSceneAsync(lvl.Scenes[0], LoadSceneMode.Single);
        scene.allowSceneActivation = false;

        loaderCanvas.SetActive(true);
        tipTextBox.text = tips[Random.Range(0, tips.Length)];

        do
        {
            await Task.Delay(100);

            target = (scene.progress / scenesSize) + 0.09f;

        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;

        if (scenesSize > 1)
        {
            for (int i = 1; i < scenesSize; i++)
            {
                var scene2 = SceneManager.LoadSceneAsync(lvl.Scenes[i], LoadSceneMode.Additive);
                scene2.allowSceneActivation = false;

                do
                {
                    await Task.Delay(100);

                    target = (scene2.progress / (scenesSize - i)) + 0.09f;

                } while (scene2.progress < 0.9f);

                scene2.allowSceneActivation = true;
            }
        }

        loaderCanvas.SetActive(false);
    }

    public async void LoadScene(string sceneName)
    {
        target = 0;
        progressBar.value = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        scene.allowSceneActivation = false;

        loaderCanvas.SetActive(true);
        tipTextBox.text = tips[Random.Range(0, tips.Length)];

        // Start checking progress
        while (!scene.isDone)
        {
            float progress = Mathf.Clamp01(scene.progress / 0.9f); // Normalize to 0-1
            target = progress;

            // Only allow activation if ready
            if (scene.progress >= 0.9f)
            {
                await Task.Delay(200); // Short optional wait (smoother transition)
                scene.allowSceneActivation = true;
            }

            await Task.Yield(); // No delay, keeps UI responsive
        }

        loaderCanvas.SetActive(false);

        //IF loadIntoLevelSelectionPanel
        //SCREAM GAMEEVENT TO OPEN IN LEVEL SELECTION PANEL IN MAINMENU CANVAS
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
