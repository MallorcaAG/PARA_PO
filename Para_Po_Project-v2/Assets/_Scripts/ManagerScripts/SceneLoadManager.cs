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
    [SerializeField] private GameEvent onLoadIntoLevelSelectionPanelIsTrue;
    private int levelInUnlockedLevels;
    [Header("References")]
    [Tooltip("ENSURE WHAT IS ASSIGNED BELOW IS THE SAME AS WHAT IS ASSIGNED IN THE LEVELSMANGER \n" +
             "(No clue what how to link these two variables together) (No clue what'll happen if not the same but I dont wanna find out)")]
    [SerializeField] private Levels[] Levels;
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI tipTextBox;

    private float target;
    private bool init = false;
    private DataManager data;


    public Levels[] getLevels()
    {
        return Levels;
    }
    public void LoadIntoLevelSelectionPanel(bool con)
    {
        loadIntoLevelSelectionPanel = con;
    }

    private void Start()
    {
        data = DataManager.Instance;

        init = true;

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

    private void ReAssignCanvasReferences()
    {
        if ((loaderCanvas == null) || (progressBar == null) || (tipTextBox == null))
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("LoadingScreen");
            foreach (GameObject obj in objs)
            {
                if ((obj.name == "LoadingScreenPanel") && (loaderCanvas == null))
                {
                    loaderCanvas = obj;
                    Debug.LogWarning("Assigning loaderCanvas");
                }

                if ((obj.name == "Slider") && (progressBar == null))
                {
                    progressBar = obj.GetComponent<Slider>();
                    Debug.LogWarning("Assigning progressBar");
                }

                if ((obj.name == "Text (TMP)") && (tipTextBox == null))
                {
                    tipTextBox = obj.GetComponent<TextMeshProUGUI>();
                    Debug.LogWarning("Assigning tipTextBox");
                }
            }
        }
    }

    public async void LoadLevelsScenes(Levels lvl, int unlockedLvlPos)  //GOD I HOPE THIS WORKS
    {
        target = 0;
        progressBar.value = 0;
        levelInUnlockedLevels = unlockedLvlPos;

        int scenesSize = lvl.Scenes.Length;

        var scene = SceneManager.LoadSceneAsync(lvl.Scenes[0], LoadSceneMode.Single);
        scene.allowSceneActivation = false;

        ReAssignCanvasReferences();

        loaderCanvas.SetActive(true);   //ISSUES
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

        await Task.Delay(500);
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

        await Task.Delay(500);
        loaderCanvas.SetActive(false);

        //IF loadIntoLevelSelectionPanel
        if(loadIntoLevelSelectionPanel)
        {
            onLoadIntoLevelSelectionPanelIsTrue.Raise(this, data.CurrentIterator);
        }
        //SCREAM GAMEEVENT TO OPEN IN LEVEL SELECTION PANEL IN MAINMENU CANVAS
    }
    #endregion

    #region DataLoadingHandling
    public void LoadLevelsData()
    {
        if(!init)
        {
            return;
        }

        Debug.Log("Loading Levels Data");

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
