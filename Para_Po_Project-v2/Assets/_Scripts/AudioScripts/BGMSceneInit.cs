using UnityEngine;

enum SceneType
{
    MainMenu, Level, Cutscene
};

public class BGMSceneInit : MonoBehaviour
{
    [SerializeField] private GameEvent onMainMenuLoaded;
    [SerializeField] private GameEvent onLevelLoaded;
    [SerializeField] private GameEvent onCutsceneLoaded;
    [SerializeField] private SceneType sceneType;

    // Start is called before the first frame update
    void Start()
    {
        switch(sceneType)
        {
            case SceneType.MainMenu:
                onMainMenuLoaded.Raise(this, 0);
                break;

            case SceneType.Level:
                onLevelLoaded.Raise(this, 0);
                break;

            case SceneType.Cutscene:
                onCutsceneLoaded.Raise(this, 0);
                break;
        }
    }

}
