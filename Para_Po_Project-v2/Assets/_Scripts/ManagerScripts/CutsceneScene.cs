using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneScene : MonoBehaviour
{
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private bool isMainMenu = false;
    [Tooltip("Load into Main Menu")]
    [SerializeField] private string sceneString = "_MainMenu";

    private VideoClip clip;
    private bool skipCutscene = false;

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

            if (isMainMenu)
            {
                _player.Stop();
                gameObject.SetActive(false);
                return;
            }

            //Stop Coroutines
            StopAllCoroutines();

            //Load Next Scene
            LoadNextScene();
        }
        
    }

    private IEnumerator SceneChanger(float duration)
    {
        yield return new WaitForSeconds(duration);

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneLoadManager.Instance.LoadScene(sceneString);
    }
}
