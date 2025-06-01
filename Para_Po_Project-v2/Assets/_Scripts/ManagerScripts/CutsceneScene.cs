using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneScene : MonoBehaviour
{
    [SerializeField] private VideoPlayer _player;
    [Tooltip("Load into Main Menu")]
    [SerializeField] private string sceneString = "_MainMenu";

    [Header("MainMenu settings")]
    [SerializeField] private bool isMainMenu = false;
    [SerializeField] private GameObject StartPanel;

    private VideoClip clip;
    private bool skipCutscene = false, loadingScene = false;

    // Start is called before the first frame update
    void Start()
    {
        clip = _player.clip;

        double clipDuration = clip.length;

        _player.Stop();
        _player.Play();

        if(isMainMenu)
        {
            return;
        }

        StartCoroutine(SceneChanger((float)clipDuration));
    }

    private void OnEnable()
    {
        _player.Play();
    }

    private void Update()
    {
        //Get ESC input
        if(Input.GetKey(KeyCode.Escape))
        {
            skipCutscene = true;
        }
        
        if(skipCutscene)
        {
            skipCutscene = false;

            _player.Stop();

            if (isMainMenu)
            {
                gameObject.SetActive(false);
                StartPanel.SetActive(true);
                return;
            }

            //Stop Coroutines
            StopAllCoroutines();

            //Load Next Scene
            if(!loadingScene)
            {
                LoadNextScene();
            }
        }
        
    }

    private IEnumerator SceneChanger(float duration)
    {
        yield return new WaitForSeconds(duration);

        skipCutscene = true;

        if (!loadingScene)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        loadingScene = true;
        SceneLoadManager.Instance.LoadScene(sceneString);
    }
}
